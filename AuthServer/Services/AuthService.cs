using Azure.Core;
using AuthServer.Data;
using AuthServer.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Shared;
using Shared.DTOs;

namespace AuthServer.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;


    private TokenService _tokenService;

    public AuthService(AppDbContext db, TokenService tokenService )
    {
        _db = db;
        _tokenService = tokenService;
    }

    public void AppendCookie(HttpResponse response, string key, string value, DateTimeOffset expires)
    { 
        response.Cookies.Append(key, value, new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // SameSite=None, Secure=true is mandatory
            SameSite = SameSiteMode.None,
            Expires = expires
        });
    }

    public async Task<User?> GetUserByLoginAsync(string login)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Login == login);
    }

    public async Task UpdateUserAsync(User user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
    }

    public async Task<ServiceResult> RefreshCheck(string? refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
            return ServiceResult.Fail("InvalidRefreshToken");

        // Find token in DB
        var tokenRecord = await _db.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == refreshToken);

        if (tokenRecord == null || tokenRecord.ExpiresAt < DateTime.UtcNow)
            return ServiceResult.Fail("InvalidRefreshToken");

        User? user = await _db.Users.FirstOrDefaultAsync(u => u.Id == tokenRecord.UserId);
        if (user == null) return ServiceResult.Fail("InvalidUser");
        if( !user.IsOnline ) return ServiceResult.Fail("UserOffline");

        UserLoginDto login = new();
        login.Id = user.Id;
        login.PwdHashed = user.PasswordHash;

        // Generate new access token
        var newAccessToken = _tokenService.GenerateAccessToken(tokenRecord.UserId);
        login.AccessToken = newAccessToken;

        // Rotate refresh token
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        tokenRecord.Token = newRefreshToken;
        tokenRecord.ExpiresAt = DateTime.UtcNow.AddDays(7);

        await _db.SaveChangesAsync();

        ServiceResult res = ServiceResult.Ok();
        res.loginUser = login;       
        res.refreshToken = newRefreshToken;
        res.tokenExpiration = tokenRecord.ExpiresAt;
        return res;

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

        //await _hub.Clients.All.SendAsync("UserLoggedIn", user.Id);

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

        //await _hub.Clients.All.SendAsync("UserLoggedOut", user.Id);

        return ServiceResult.Ok();
    }

}
