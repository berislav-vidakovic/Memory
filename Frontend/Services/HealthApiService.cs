using Shared.DTOs;
using System.Net.Http.Json;

namespace Frontend.Services;

public class HealthApiService
{
    private readonly HttpClient _http;

    public HealthApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<HealthResponseDto?> GetHealthAsync()
    {
        return await _http.GetFromJsonAsync<HealthResponseDto>("https://localhost:5206/api/health");
    }
}
