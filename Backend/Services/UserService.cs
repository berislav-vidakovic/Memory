using Backend.Data;
using Backend.Hubs;
using Backend.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.DTOs;

namespace Backend.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    private readonly IHubContext<NotificationHub> _hub;

    public UserService(AppDbContext db, IHubContext<NotificationHub> hub)
    {
        _db = db;
        _hub = hub;
    }

    public async Task<ServiceResult> DeleteAsync(UserLoginDto userDto)
    {
        // Find user by Login
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userDto.Id);

        if (user == null)        
            return ServiceResult.Fail("UserNotFound");        
        else if( user.IsOnline )
            return ServiceResult.Fail("UserIsOnline");

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();

        Console.WriteLine($"User '{user.Id}' deleted");

        await _hub.Clients.All.SendAsync("UserDeleted", userDto.Id);

        return ServiceResult.Ok();
    }
    public async Task<ServiceResult> EditAsync(UserDto userDto)
    {
        // Find user by Login
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userDto.Id);

        if (user == null)
        {
            Console.WriteLine($"Edit user failed: user ID '{userDto.Id}' not found");
            return ServiceResult.Fail("UserNotFound");
        }

        if (userDto.IsPasswordUpdated)
        {
            user.PasswordHash = HashUtil.HashPasswordServer(userDto.HashedPwd);
            //Console.WriteLine("Password hashed server: " + user.PasswordHash);
        }

        user.Login = userDto.Login;
        user.FullName = userDto.FullName;

        await _db.SaveChangesAsync();

        Console.WriteLine($"User '{user.Login}' updated");

        // Map back to DTO to return
        var updatedDto = new UserDto
        {
            Id = user.Id,
            Login = user.Login,
            FullName = user.FullName,
            HashedPwd = user.PasswordHash,
            IsPasswordUpdated = userDto.IsPasswordUpdated
        };

        await _hub.Clients.All.SendAsync("UserUpdated", updatedDto);

        ServiceResult res = ServiceResult.Ok();
        res.user = updatedDto;
        return res;
    }

    public async Task<ServiceResult> CreateAsync(UserDto userDto)
    {
        // Check existing users by Login
        if (await _db.Users.FirstOrDefaultAsync(u => u.Login == userDto.Login) != null)
        {
            Console.WriteLine($"Create user failed: user login exists");
            return ServiceResult.Fail("UserExists");
        }

        User user = new() {
            PasswordHash = HashUtil.HashPasswordServer(userDto.HashedPwd),
            Login = userDto.Login,
            FullName = userDto.FullName
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        Console.WriteLine($"User '{user.Login}' created");

        // Map back to DTO to return
        var updatedDto = new UserDto
        {
            Id = user.Id,
            Login = user.Login,
            FullName = user.FullName,
            HashedPwd = user.PasswordHash,
        };

        await _hub.Clients.All.SendAsync("UserCreated", updatedDto);

        ServiceResult res = ServiceResult.Ok();
        res.user = updatedDto;
        return res;
    }


    public async Task<ServiceResult> GetAllUsersAsync()
    {
        var users = await _db.Users
            .Select(u => new UserDto
            {
                FullName = u.FullName,
                IsOnline = u.IsOnline,
                Login = u.Login,
                Id = u.Id,
                HashedPwd = u.PasswordHash
            })
            .ToListAsync();

        ServiceResult res = ServiceResult.Ok();  
        res.allUsers = users;
        return res;
    }
}
