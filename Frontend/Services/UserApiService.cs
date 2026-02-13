using Frontend.Models;
using Microsoft.JSInterop;
using Shared.DTOs;
using System.Collections.Generic;
using System.Net.Http.Json;

namespace Frontend.Services;

public class UserApiService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;

    public UserApiService(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js = js; 
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
    

    public async Task<UserDto?> CreateUserAsync(UserDto dto)
    {
        // Send POST to /api/edituser
        var response = await _http.PostAsJsonAsync("https://localhost:5206/api/createuser", dto);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Create user successful!");
            var newUser = await response.Content.ReadFromJsonAsync<UserDto>();
            return newUser;
        }
        else
        {
            Console.WriteLine($"Create user failed: {response.StatusCode}");
            return null;
        }
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


 
    public async Task<bool> DeleteAsync(UserLoginDto login)
    {
        // Send POST to /api/login
        var response = await _http.PostAsJsonAsync("https://localhost:5206/api/deleteuser", login);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Deletion successful!");
            return true;
        }
        else
        {
            Console.WriteLine($"Deletion failed: {response.StatusCode}");
            return false;
        }
    }

   
}
