using Shared.DTOs;
using System;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace Frontend.Services
{
    // Handle  Login, Logout and RefreshCheck
    public class AuthService
    {
        // Access token stored in memory
        private string? _accessToken;

        private readonly JsCookiesService _jsCookiesService;

        public AuthService(JsCookiesService jsCookiesService )
        {
            _jsCookiesService = jsCookiesService;
        }

        public string? AccessToken => _accessToken;

        // Set or update the access token
        private void SetAccessToken(string token)
        {
            _accessToken = token;
        }


        // Clear token on logout
        private void ClearAccessToken()
        {
            _accessToken = null;
        }

        // check if user is authenticated
        public bool IsAuthenticated => !string.IsNullOrEmpty(_accessToken);

        // on browser refresh...
        
        public async Task<UserLoginDto?> RestoreSessionAsync()
        {
            try
            {
                var res = await _jsCookiesService.PostAsync<UserLoginDto>(
                        "https://localhost:7200/api/refreshcheck", null );

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
                var dtoResponse = await _jsCookiesService.PostAsync<UserLoginDto>(
                        "https://localhost:7200/api/login",
                        login
                    );
                if (dtoResponse != null)
                {
                    Console.WriteLine("Login successful!");
                    Console.WriteLine("Access Token from backend: " + dtoResponse.AccessToken);
                    SetAccessToken(dtoResponse.AccessToken);
                }


                return dtoResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login failed: {ex.Message}");
                return null;
            }
        }

        public async Task<UserLoginDto?> LogoutAsync(UserLoginDto login)
        {
            try
            {
                var dto = await _jsCookiesService.PostAsync<UserLoginDto>(
                        "https://localhost:7200/api/logout",
                        login
                    );

                if( dto != null )
                {
                    Console.WriteLine("Logout successful!");
                    ClearAccessToken();
                }
                return dto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logout failed: {ex.Message}");
                return null;
            }
        }

    }
}
