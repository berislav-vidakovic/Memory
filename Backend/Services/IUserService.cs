using Shared.DTOs;

namespace Backend.Services
{
    public interface IUserService
    {
        Task<ServiceResult> DeleteAsync(UserLoginDto login);
        Task<ServiceResult> EditAsync(UserDto login);

        Task <ServiceResult> GetAllUsersAsync();
    }
}
