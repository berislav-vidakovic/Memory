using Frontend.Services;
using Shared.DTOs;

namespace Frontend.ViewModels;

/*
MVVM Architecture

-ViewModel owns UI state
-It decides what message to show
-UI just renders whatever ViewModel exposes
*/

public class HealthViewModel
{
    private readonly HealthApiService _healthService;
    private readonly ChatSignalRService _chatService;

    public string StatusMessage { get; private set; } = "Checking backend...";
    public string CurrentMessage { get; set; } = "";

    public HealthResponseDto? Health { get; private set; }

    public event Action? OnStateChanged;


    public HealthViewModel(
        HealthApiService healthService,
        ChatSignalRService chatService)
    {
        _healthService = healthService;
        _chatService = chatService;

        _chatService.OnMessageReceived += msg =>
        {
            CurrentMessage = msg;
            OnStateChanged?.Invoke(); // notify UI
        };
    }

    public async Task LoadHealthAsync()
    {
        Health = await _healthService.GetHealthAsync();
        //StatusMessage = Health != null
        //  ? $"...Connected to backend! <br />Backend is running on https://localhost:5206"
        // : "Failed to connect to backend";

        if (Health != null)
        {
            StatusMessage = Health.Status;
            StatusMessage += "<br />";
            StatusMessage += Health.DBmessage;
        }
        else
            StatusMessage = "(no backend connection)";


            OnStateChanged?.Invoke();
    }

    public async Task StartChatAsync()
    {
        await _chatService.StartAsync();
    }

    public async Task SendMessageAsync()
    {
        await _chatService.SendMessageAsync(CurrentMessage);
    }
}
