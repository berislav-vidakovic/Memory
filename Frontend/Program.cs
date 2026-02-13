using Frontend;
using Frontend.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add HttpClient
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:5206") });


//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


// Register Services
builder.Services.AddScoped<HealthApiService>();
builder.Services.AddScoped<UserApiService>();

builder.Services.AddSingleton<ChatSignalRService>();
builder.Services.AddSingleton<NotificationService>();

builder.Services.AddScoped<JsCookiesService>();

builder.Services.AddScoped<AuthService>();

var host = builder.Build();

//var auth = host.Services.GetRequiredService<AuthService>();
//var http = host.Services.GetRequiredService<HttpClient>();

//await auth.RestoreSessionAsync(http);

await host.RunAsync();
