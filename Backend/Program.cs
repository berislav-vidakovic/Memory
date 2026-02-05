using Shared.DTOs;
using Backend.Data;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);


// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer("Server=barryonweb.com,1433;Database=Memory;User Id=SA;Password=Abc1234!;TrustServerCertificate=True;"));


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

// Enable OpenAPI (built-in .NET 10 support)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Test backend
// https://localhost:5206/swagger
// https://localhost:5206/swagger/v1/swagger.json

builder.Services.AddSignalR();



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
    Console.WriteLine("============Hello world from Dev Env!===============");
}

app.UseHttpsRedirection();

app.MapHub<Backend.Hubs.ChatHub>("/hubs/chat");


// Startup logic: read first Health record




app.MapGet("/api/health", async (IServiceProvider services) =>
{
    HealthResponseDto resp = new()
    {
        Status = "Backend is running",
        Timestamp = DateTime.UtcNow
    };

    using var scope = services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var firstHealth = await db.Healths.FirstOrDefaultAsync();

    if (firstHealth != null)
        resp.DBmessage = firstHealth.Msg;
    else
        resp.DBmessage = "No health DB records found";

    return Results.Ok(resp);
});

app.MapGet("/api/users", async (IServiceProvider services) =>
{
    UsersResponseDto resp = new()
    {
        Status = "Users are created",
        Timestamp = DateTime.UtcNow
    };
    /*
    using var scope = services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var firstHealth = await db.Healths.FirstOrDefaultAsync();

    if (firstHealth != null)
        resp.DBmessage = firstHealth.Msg;
    else
        resp.DBmessage = "No health DB records found";
    */

    return Results.Ok(resp);
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
