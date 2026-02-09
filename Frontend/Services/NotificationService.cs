using Microsoft.AspNetCore.SignalR.Client;

namespace Frontend.Services;

public class NotificationService
{
    private HubConnection? _connection;


    public async Task StartAsync()
    {
        Console.WriteLine("Starting SignalR connection...");

        _connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:5206/hubs/notification")
            .WithAutomaticReconnect()
            .Build();

        
        await _connection.StartAsync();
        Console.WriteLine("SignalR connected!");
    }
 
}
