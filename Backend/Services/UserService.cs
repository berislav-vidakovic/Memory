using Backend.Data;
using Backend.Hubs;
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

    public async Task<ServiceResult> DeleteAsync(UserLoginDto login)
    {
        return ServiceResult.Ok();

    }
    public async Task<ServiceResult> EditAsync(UserDto login)
    {
        return ServiceResult.Ok();

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
        res.users = users;
        return res;
    }
}
