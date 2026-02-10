using Shared.DTOs;

namespace Backend.Services
{
    public class ServiceResult
    {
        public bool Success { get; init; }
        public string? Error { get; init; }

        public static ServiceResult Ok() => new() { Success = true };
        public static ServiceResult Fail(string error) => new() { Success = false, Error = error };

        public List<UserDto>? allUsers = null;
        public UserDto? user = null;
        public UserLoginDto? loginUser = null;
        public string refreshToken = string.Empty;
        public DateTime tokenExpiration;
    }

}
