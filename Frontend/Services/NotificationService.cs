using Microsoft.AspNetCore.SignalR.Client;

namespace Frontend.Services;

public class NotificationService
{
    private HubConnection? _connection;

    public event Action<int>? OnUserLoggedIn;

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

        await _connection.StartAsync();
        Console.WriteLine("SignalR connected!");
    }
 
}
