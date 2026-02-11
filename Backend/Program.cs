using Backend.Data;
using Backend.Hubs;
using Backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shared;
using Shared.DTOs;


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

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<TokenService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:7173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // mandatory for cookies
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

app.MapHub<NotificationHub>("/hubs/notification");




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

app.MapGet("/api/users", async (IUserService userService) =>
{
    ServiceResult result = await userService.GetAllUsersAsync();

    if (result.Success)
        return Results.Ok(result.allUsers);

     return Results.NotFound();
});

app.MapPost("/api/edituser", async (IUserService userService, UserDto userDto) =>
{
    ServiceResult result = await userService.EditAsync(userDto);

    if (result.Success)
        return Results.Ok(result.user);

    return Results.NotFound();
});

app.MapPost("/api/createuser", async (IUserService userService, UserDto userDto) =>
{
    ServiceResult result = await userService.CreateAsync(userDto);

    if (result.Success)
        return Results.Ok(result.user);

    return Results.NotFound();
});


app.MapPost("/api/deleteuser", async (IUserService userService, UserLoginDto userDto) =>
{
    var result = await userService.DeleteAsync(userDto);

    if (result.Success)
        return Results.Ok();

    switch (result.Error)
    {
        case "UserNotFound":
            return Results.NotFound();       
        default:
            return Results.BadRequest("Cannot delete user that is online");
    };
});

app.MapPost("/api/refreshcheck", async (IAuthService auth, HttpRequest request, HttpResponse response) =>
{
    // Read refresh token from HttpOnly cookie
    string? refreshToken = request.Cookies["refreshToken"];

    ServiceResult result = await auth.RefreshCheck(refreshToken);

    if (!result.Success)
        switch (result.Error)
        {
            case "InvalidRefreshToken":
                return Results.Unauthorized();
            default:
                return Results.BadRequest();
        };
      

    // Update HttpOnly cookie
    response.Cookies.Append("refreshToken", result.refreshToken, new CookieOptions
    {
        HttpOnly = true,
        Secure = false,
        SameSite = SameSiteMode.None,
        Expires = result.tokenExpiration
    });

    return Results.Ok(result.loginUser);

});


app.MapPost("/api/loginrefresh", async (IAuthService auth, HttpResponse response) =>
{
    // Set refresh token cookie
    var refreshToken = Guid.NewGuid().ToString();
    response.Cookies.Append("X-Refresh-Token", refreshToken, new CookieOptions
    {
        HttpOnly = true,
        Secure = false,
        SameSite = SameSiteMode.None,
        Expires = DateTime.UtcNow.AddDays(7)
    });

    return Results.Ok();
});

app.MapPost("/api/login", async (IAuthService auth, HttpResponse response, UserLoginDto login) =>
{
    var result = await auth.LoginAsync(login);

    if (!result.Success)
        switch (result.Error)
        {
            case "UserNotFound":
                return Results.NotFound();
            case "InvalidPassword":
                return Results.Unauthorized();
            default:
                return Results.BadRequest();
        };

    // Set refresh token cookie
    var refreshToken = await auth.GetRefreshTokenForUser(login.Id); // method to get latest token
    response.Cookies.Append("X-Refresh-Token", refreshToken, new CookieOptions
    {
        HttpOnly = true,
        Secure = false,
        SameSite = SameSiteMode.None,
        Expires = DateTime.UtcNow.AddDays(7)
    });

    return Results.Ok(result.loginUser);
});


app.MapPost("/api/logout", async (IAuthService auth, UserLoginDto login) =>
{
    var result = await auth.LogoutAsync(login);

    if (result.Success)
        return Results.Ok();

    switch (result.Error)
    {
        case "UserNotFound":
            return Results.NotFound();
        default:
            return Results.BadRequest();
    };
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
