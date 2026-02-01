using Frontend.Services;
using Shared.DTOs;

namespace Frontend.ViewModels;

public class HealthViewModel
{
    private readonly HealthApiService _service;

    public HealthResponseDto? Health { get; private set; }

    public HealthViewModel(HealthApiService service)
    {
        _service = service;
    }

    public async Task LoadHealthAsync()
    {
        Health = await _service.GetHealthAsync();
        Console.WriteLine(Health != null
            ? $"Connected to backend! Backend is running on https://localhost:5206"
            : "Failed to connect to backend");
    }
}
