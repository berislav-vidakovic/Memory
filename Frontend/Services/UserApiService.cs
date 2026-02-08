using Frontend.Models;
using Shared.DTOs;
using System.Collections.Generic;
using System.Net.Http.Json;

namespace Frontend.Services;

public class UserApiService
{
    private readonly HttpClient _http;

    public UserApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<User>> GetUsersAsync()
    {
        var dtos = await _http.GetFromJsonAsync<List<UserDto>>("https://localhost:5206/api/users");
        if( dtos != null )
            return dtos.Select(dto =>
            {
                var user = new User();
                user.MapFromDto(dto);
                return user;
            }).ToList();

        return new List<User>();
    }

    public async Task<UserDto?> EditUserAsync(UserDto dto)
    {
        // Send POST to /api/edituser
        var response = await _http.PostAsJsonAsync("https://localhost:5206/api/edituser", dto);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Edit user successful!");
            var updatedUser = await response.Content.ReadFromJsonAsync<UserDto>();
            return updatedUser;
        }
        else
        {
            Console.WriteLine($"Edit user failed: {response.StatusCode}");
            return null;
        }
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

    public async Task<bool> LogoutAsync(UserLoginDto login)
    {
        // Send POST to /api/logout
        var response = await _http.PostAsJsonAsync("https://localhost:5206/api/logout", login);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Logout successful!");
            return true;
        }
        else
        {
            Console.WriteLine($"Logout failed: {response.StatusCode}");
            return false;
        }
    }
}
