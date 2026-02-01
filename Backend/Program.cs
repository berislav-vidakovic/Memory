using Shared.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

// Enable OpenAPI (built-in .NET 10 support)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Test backend
// https://localhost:5206/swagger
// https://localhost:5206/swagger/v1/swagger.json




builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:7173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
   

var app = builder.Build();

app.UseCors("FrontendPolicy");



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // In .NET 10, MapOpenApi automatically exposes JSON + UI
    //app.MapOpenApi();  // serves both the OpenAPI JSON and the classic UI
    app.UseSwagger();               // serves JSON at /swagger/v1/swagger.json
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/health", () =>
{
    return Results.Ok(new HealthResponseDto
    {
        Status = "Backend is running",
        Timestamp = DateTime.UtcNow
    });
});

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
