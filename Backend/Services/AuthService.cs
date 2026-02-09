using Backend.Data;
using Backend.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.DTOs;

namespace Backend.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;

    private readonly IHubContext<NotificationHub> _hub;

    public AuthService(AppDbContext db, IHubContext<NotificationHub> hub)
    {
        _db = db;
        _hub = hub;
    }

    public async Task<ServiceResult> LoginAsync(UserLoginDto login)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == login.Id);

        if (user == null)
            return ServiceResult.Fail("UserNotFound");

        // First-time login
        if (string.IsNullOrEmpty(user.PasswordHash))
        {
            user.PasswordHash = HashUtil.HashPasswordServer(login.PwdHashed);
        }
        else
        {
            bool valid = HashUtil.VerifyPasswordServer(login.PwdHashed, user.PasswordHash);
            if (!valid)
                return ServiceResult.Fail("InvalidPassword");
        }

        user.IsOnline = true;
        await _db.SaveChangesAsync();

        await _hub.Clients.All.SendAsync("UserLoggedIn", user.Id);

        return ServiceResult.Ok();
    }

}
