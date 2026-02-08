using Shared.DTOs;
using Shared;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


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
    
    using var scope = services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    var users = await db.Users
        .Select(u => new UserDto
        {
            FullName = u.FullName,
            IsOnline = u.IsOnline,
            Login = u.Login,
            Id = u.Id,
            HashedPwd = u.PasswordHash
        })
        .ToListAsync();
    

    return Results.Ok(users);
});

app.MapPost("/api/edituser", async (IServiceProvider services, UserDto userDto) =>
{
    //Console.WriteLine($"Edit user attempt: {user.Login}, PwdHashedClient: {login.PwdHashed}");

    using var scope = services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Find user by Login
    var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userDto.Id);

    if (user == null)
    {
        Console.WriteLine($"Edit user failed: user ID '{userDto.Id}' not found");
        return Results.NotFound("User not found");
    }

    if (userDto.IsPasswordUpdated)
    {
        user.PasswordHash = HashUtil.HashPasswordServer(userDto.HashedPwd);
        //Console.WriteLine("Password hashed server: " + user.PasswordHash);
    }      

    user.Login = userDto.Login;
    user.FullName = userDto.FullName;

    await db.SaveChangesAsync();

    Console.WriteLine($"User '{user.Login}' updated");

    // Return OK always
    return Results.Ok();
});



app.MapPost("/api/login", async (IServiceProvider services, UserLoginDto login) =>
{
    // Just log what came in for testing
    Console.WriteLine($"Login attempt: {login.Login}, PwdHashedClient: {login.PwdHashed}");

    using var scope = services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Find user by Login
    var user = await db.Users.FirstOrDefaultAsync(u => u.Login == login.Login);

    if (user == null)
    {
        Console.WriteLine($"Login failed: user '{login.Login}' not found");
        return Results.NotFound("User not found");
    }

    if (string.IsNullOrEmpty(user.PasswordHash))  // 1st time login    
    {
        user.PasswordHash = HashUtil.HashPasswordServer(login.PwdHashed);
        Console.WriteLine("Password hashed server: " + user.PasswordHash);
    }
    else
    {
        // Password verification
        bool valid = HashUtil.VerifyPasswordServer(login.PwdHashed, user.PasswordHash);

        if (!valid)
            return Results.Unauthorized();
    }

    user.IsOnline = true;

    await db.SaveChangesAsync();

    Console.WriteLine($"User '{user.Login}' set to online");

    // Return OK always
    return Results.Ok();
});

app.MapPost("/api/logout", async (IServiceProvider services, UserLoginDto login) =>
{
    // Just log what came in for testing
    Console.WriteLine($"Logou attempt: {login.Login}");

    using var scope = services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Find user by Login
    var user = await db.Users.FirstOrDefaultAsync(u => u.Login == login.Login);

    if (user == null)
    {
        Console.WriteLine($"Logout failed: user '{login.Login}' not found");
        return Results.NotFound("User not found");
    }

    user.IsOnline = false;
    await db.SaveChangesAsync();

    Console.WriteLine($"User '{user.Login}' set to offline");

    // Return OK always
    return Results.Ok();
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
