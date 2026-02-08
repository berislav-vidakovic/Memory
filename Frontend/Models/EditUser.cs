namespace Frontend.Models
{
    public class EditUser
    {
        public int Id { get; set; }

        public string Login { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;

        public bool IsPasswordUpdated { get; set; } = false;
    }

}
