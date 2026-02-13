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

        private readonly JsCookiesService _jsCookiesService;

        public AuthService(JsCookiesService jsCookiesService )
        {
            _jsCookiesService = jsCookiesService;
        }

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

        public async Task<UserLoginDto?> RestoreSessionAsync()
        {
            try
            {
                var res = await _jsCookiesService.PostAsync<UserLoginDto>(
                        "https://localhost:5206/api/refreshcheck", null );

                Console.WriteLine("RefreshCheck done!");
                if( res != null )
                {
                    SetAccessToken(res.AccessToken);
                }
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RefreshCheck failed: {ex.Message}");
                return null;
            }
        }

        public async Task<UserLoginDto?> LoginAsync(UserLoginDto login)
        {
            try
            {
                var dto = await _jsCookiesService.PostAsync<UserLoginDto>(
                        "https://localhost:5206/api/login",
                        login
                    );

                Console.WriteLine("Login successful!");
                return dto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login failed: {ex.Message}");
                return null;
            }
        }


    }
}
