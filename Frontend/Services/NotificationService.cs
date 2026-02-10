using Microsoft.AspNetCore.SignalR.Client;
using Shared.DTOs;

namespace Frontend.Services;

public class NotificationService
{
    private HubConnection? _connection;

    public event Action<int>? OnUserLoggedIn;
    public event Action<int>? OnUserLoggedOut;
    public event Action<UserDto>? OnUserUpdated;
    public event Action<int>? OnUserDeleted;
    public event Action<UserDto>? OnUserCreated;


    public async Task StartAsync()
    {
        Console.WriteLine("Starting SignalR connection...");

        _connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:5206/hubs/notification")
            .WithAutomaticReconnect()
            .Build();

        _connection.On<int>("UserLoggedIn", userId =>
        {
            Console.WriteLine($"SignalR: User logged in: {userId}");
            OnUserLoggedIn?.Invoke(userId);
        });

        _connection.On<int>("UserLoggedOut", userId =>
        {
            Console.WriteLine($"SignalR: User logged out: {userId}");
            OnUserLoggedOut?.Invoke(userId);
        });

        _connection.On<UserDto>("UserUpdated", userDto =>
        {
            Console.WriteLine($"SignalR: User update: {userDto.Id}");
            OnUserUpdated?.Invoke(userDto);
        });

        _connection.On<int>("UserDeleted", userId =>
        {
            Console.WriteLine($"SignalR: User deleted: {userId}");
            OnUserDeleted?.Invoke(userId);
        });

        _connection.On<UserDto>("UserCreated", userId =>
        {
            Console.WriteLine($"SignalR: User created: {userId}");
            OnUserCreated?.Invoke(userId);
        });

        await _connection.StartAsync();
        Console.WriteLine("SignalR connected!");
    }
 
}
