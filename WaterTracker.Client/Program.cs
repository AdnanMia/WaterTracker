using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WaterTracker.Client;
using WaterTracker.Client.Services.Authentication;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"]
    ?? throw new InvalidOperationException("ApiSettings:BaseUrl is not configured.");

builder.Services.AddScoped<ITokenStorageService, TokenStorageService>();

builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
    client.BaseAddress = new Uri(apiBaseUrl.TrimEnd('/') + '/'));

await builder.Build().RunAsync();
