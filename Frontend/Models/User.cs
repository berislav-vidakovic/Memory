using Shared.DTOs;

namespace Frontend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string HashedPwd { get; set; } = string.Empty;
        public bool IsOnline { get; set; }

        public void MapFromDto(UserDto dto)
        {
            Id = dto.Id;
            FullName = dto.FullName;
            Login = dto.Login;
            HashedPwd = dto.HashedPwd;
            IsOnline = dto.IsOnline;
        }
        /*
        private string _login = "";

        public string Login
        {
            get => _login;
            set
            {
                Console.WriteLine($"LOGIN CHANGED: {value}");
                Console.WriteLine(Environment.StackTrace);
                _login = value;
            }
        }*/
    }
}
