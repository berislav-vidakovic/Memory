using Microsoft.AspNetCore.SignalR.Client;

namespace Frontend.Services;

public class ChatSignalRService
{
    private HubConnection? _connection;

    public event Action<string>? OnMessageReceived;

    public async Task StartAsync()
    {
        Console.WriteLine("Starting SignalR connection...");

        _connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:5206/hubs/chat")
            .WithAutomaticReconnect()
            .Build();

        _connection.On<string>("ReceiveMessage", message =>
        {
            Console.WriteLine($"Message received: {message}");
            OnMessageReceived?.Invoke(message);
        });

        await _connection.StartAsync();
        Console.WriteLine("SignalR connected!");
    }

    public async Task SendMessageAsync(string message)
    {
        if (_connection != null)
        {
            await _connection.SendAsync("SendMessage", message);
        }
    }
}
