using Frontend.Models;

namespace Frontend
{
    public class AppState
    {
        // ====== Users ======
        public List<User> Users { get; set; } = new();

        // ====== Auth ======
        public int? CurrentUserId { get; private set; }

        // ====== Events ======
        public event Action? OnChange;

        // ====== Methods ======
        public void SetUsers(List<User> users)
        {
            Users = users;
            NotifyStateChanged();
        }

        public void SetCurrentUser(int? userId)
        {
            CurrentUserId = userId;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
