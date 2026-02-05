using Frontend;
using Frontend.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add HttpClient
builder.Services.AddScoped(sp => new HttpClient());
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


// Register Services
builder.Services.AddScoped<HealthApiService>();
builder.Services.AddScoped<UserApiService>();

builder.Services.AddSingleton<ChatSignalRService>();



await builder.Build().RunAsync();
