using Shared.DTOs;
using System;
using System.Net.Http.Json;

namespace Frontend.Services
{
    public class AuthService
    {
        // Access token stored in memory
        private string? _accessToken;
        private int? _currentUserId;

        public string? AccessToken => _accessToken;

        // Set or update the access token
        public void SetAccessToken(string token)
        {
            _accessToken = token;
        }

        public void SetCurrentUserId(int id)
        {
            _currentUserId = id;
        }

        public int?  GetCurrentUserId()
        {
            return _currentUserId;
        }



        // Clear token on logout
        public void ClearAccessToken()
        {
            _accessToken = null;
            _currentUserId=null;
        }

        // Simple helper to check if user is authenticated
        public bool IsAuthenticated => !string.IsNullOrEmpty(_accessToken);

        // on browser refresh...
        public async Task<bool> RestoreSessionAsync(HttpClient http)
        {
            try
            {
                var response = await http.PostAsync("/api/refreshcheck", null);

                if (!response.IsSuccessStatusCode)
                    return false;

                UserLoginDto? dto = await response.Content.ReadFromJsonAsync<UserLoginDto>();

                if (dto?.AccessToken == null)
                    return false;

                SetAccessToken(dto.AccessToken);
                SetCurrentUserId(dto.Id);
                Console.WriteLine("Session restored. New access token set.");

                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
