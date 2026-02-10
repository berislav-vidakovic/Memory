using Backend.Data;
using Backend.Hubs;
using Backend.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.DTOs;

namespace Backend.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;

    private readonly IHubContext<NotificationHub> _hub;

    private TokenService _tokenService;

    public AuthService(AppDbContext db, IHubContext<NotificationHub> hub, TokenService tokenService )
    {
        _db = db;
        _hub = hub;
        _tokenService = tokenService;
    }

    public async Task<ServiceResult> LoginAsync(UserLoginDto login)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == login.Id);

        if (user == null)
            return ServiceResult.Fail("UserNotFound");

        // First-time login
        if (string.IsNullOrEmpty(user.PasswordHash))
            user.PasswordHash = HashUtil.HashPasswordServer(login.PwdHashed);
        else if( !HashUtil.VerifyPasswordServer(login.PwdHashed, user.PasswordHash) )
            return ServiceResult.Fail("InvalidPassword");

        user.IsOnline = true;
        await _db.SaveChangesAsync();

        await _hub.Clients.All.SendAsync("UserLoggedIn", user.Id);

        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(user.Id);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Save refresh token to DB
        var rt = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.UserId == user.Id);
        if (rt == null)
        {
            rt = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };
            _db.RefreshTokens.Add(rt);
        }
        else
        {
            rt.Token = refreshToken;
            rt.ExpiresAt = DateTime.UtcNow.AddDays(7);
        }
        await _db.SaveChangesAsync();



        ServiceResult res = ServiceResult.Ok();
        res.loginUser = login;
        res.loginUser.AccessToken = accessToken;
        return res;

    }

    public async Task<string> GetRefreshTokenForUser(int userId)
    {
        // Try to get latest active token
        var tokenRecord = await _db.RefreshTokens
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.ExpiresAt)
            .FirstOrDefaultAsync(t => t.ExpiresAt > DateTime.UtcNow);

        // If none exists, create a new one
        if (tokenRecord == null)
        {
            // Generate new token (TokenService injected)
            var newToken = _tokenService.GenerateRefreshToken();

            tokenRecord = new RefreshToken
            {
                UserId = userId,
                Token = newToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            _db.RefreshTokens.Add(tokenRecord);
            await _db.SaveChangesAsync();
        }

        // Return token (non-null guaranteed)
        return tokenRecord.Token;
    }


    public async Task<ServiceResult> LogoutAsync(UserLoginDto login)
    {

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == login.Id);

        if (user == null)
            return ServiceResult.Fail("UserNotFound");


        user.IsOnline = false;
        await _db.SaveChangesAsync();

        Console.WriteLine($"User '{user.Login}' set to offline");

        await _hub.Clients.All.SendAsync("UserLoggedOut", user.Id);

        return ServiceResult.Ok();
    }

}
