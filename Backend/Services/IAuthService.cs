using Shared.DTOs;

namespace Backend.Services
{
    public interface IAuthService
    {
        Task<ServiceResult> LoginAsync(UserLoginDto login);
    }
}
