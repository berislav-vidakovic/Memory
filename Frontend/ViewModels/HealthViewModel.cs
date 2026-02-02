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
    private readonly HealthApiService _service;

    public string StatusMessage { get; private set; } = "Checking backend...";

    public HealthResponseDto? Health { get; private set; }

    public HealthViewModel(HealthApiService service)
    {
        _service = service;
    }

    public async Task LoadHealthAsync()
    {
        Health = await _service.GetHealthAsync();
        StatusMessage = Health != null
            ? $"...Connected to backend! <br />Backend is running on https://localhost:5206"
            : "Failed to connect to backend";
    }
}
