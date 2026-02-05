using Shared.DTOs;
using System.Net.Http.Json;

namespace Frontend.Services;

public class UserApiService
{
    private readonly HttpClient _http;

    public UserApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<UsersResponseDto>> GetUsersAsync()
    {
        return await _http.GetFromJsonAsync<List<UsersResponseDto>>("https://localhost:5206/api/users");
    }

    public async Task<bool> LoginAsync(UserLoginDto login)
    {
        // Send POST to /api/login
        var response = await _http.PostAsJsonAsync("https://localhost:5206/api/login", login);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Login successful!");
            return true;
        }
        else
        {
            Console.WriteLine($"Login failed: {response.StatusCode}");
            return false;
        }
    }
}
