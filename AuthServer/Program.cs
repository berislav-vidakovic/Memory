using AuthServer.Data;
using AuthServer.Services;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using OpenIddict.Abstractions;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddScoped<IAuthService, AuthService>();
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

//OIDC
builder.Services.AddOpenIddict()
    // Core
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
               .UseDbContext<AppDbContext>();
    })
    // Server
    .AddServer(options =>
    {
        options.SetAuthorizationEndpointUris("/connect/authorize");
        options.SetTokenEndpointUris("/connect/token");

        options.AllowAuthorizationCodeFlow()
               .RequireProofKeyForCodeExchange();

        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();

        options.UseAspNetCore()
               .EnableAuthorizationEndpointPassthrough()
               .EnableTokenEndpointPassthrough();
    }
 );



// Add DbContext
var connectionString = builder.Configuration["Sites:Dev"];
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.UseOpenIddict();
});


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

// Enable OpenAPI (built-in .NET 10 support)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();
app.UseCors("FrontendPolicy");





app.MapPost("/api/refreshcheck", async (IAuthService auth, HttpRequest request, HttpResponse response) =>
{
    // Read refresh token from HttpOnly cookie
    string? refreshToken = request.Cookies["X-Refresh-Token"];

    ServiceResult result = await auth.RefreshCheck(refreshToken);

    if (!result.Success)
        switch (result.Error)
        {
            case "InvalidRefreshToken":
                return Results.Unauthorized();
            case "UserOffline":
                return Results.Conflict();
            default:
                return Results.BadRequest();
        }
    ;

    // Update HttpOnly cookie
    auth.AppendCookie(response, "X-Refresh-Token", result.refreshToken, DateTime.UtcNow.AddDays(7));

    return Results.Ok(result.loginUser);

});




app.MapPost("/api/logout", async (IAuthService auth, HttpResponse response, UserLoginDto login) =>
{
    var result = await auth.LogoutAsync(login);

    if (result.Success)
    {
        auth.AppendCookie(response, "X-Refresh-Token", "", DateTime.UtcNow.AddDays(-1));
        return Results.Ok(login);
    }

    switch (result.Error)
    {
        case "UserNotFound":
            return Results.NotFound();
        default:
            return Results.BadRequest();
    }
    ;
});




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();               // serves JSON at /swagger/v1/swagger.json
    app.UseSwaggerUI();
    Console.WriteLine("============Hello world from Dev Env!===============");
}

app.UseHttpsRedirection();

app.UseRouting();

//OIDC
app.UseAuthentication();
app.UseAuthorization();


app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
