using Shared.DTOs;

namespace AuthServer.Services
{
    public interface IAuthService
    {
        Task<ServiceResult> LoginAsync(UserLoginDto login);
        Task<ServiceResult> LogoutAsync(UserLoginDto login);
        Task<string> GetRefreshTokenForUser(int userId);
        Task<ServiceResult> RefreshCheck(string? refreshToken);
        void AppendCookie(HttpResponse response, string key, string value, DateTimeOffset expires);


    }
}