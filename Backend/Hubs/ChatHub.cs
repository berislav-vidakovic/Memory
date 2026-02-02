using Microsoft.AspNetCore.SignalR;

namespace Backend.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(string message)
    {
        Console.WriteLine($"Received message: {message}");
        await Clients.All.SendAsync("ReceiveMessage", message);
    }
}
