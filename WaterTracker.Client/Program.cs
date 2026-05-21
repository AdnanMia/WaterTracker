using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WaterTracker.Client;
using WaterTracker.Client.Infrastructure;
using WaterTracker.Client.Services.Authentication;
using WaterTracker.Client.Services.WaterIntake;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"]
    ?? throw new InvalidOperationException("ApiSettings:BaseUrl is not configured.");

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ITokenStorageService, TokenStorageService>();
builder.Services.AddScoped<TokenAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<TokenAuthenticationStateProvider>());

builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
    client.BaseAddress = new Uri(apiBaseUrl.TrimEnd('/') + '/'));

builder.Services.AddTransient<AuthHeaderHandler>();

builder.Services.AddHttpClient<IWaterIntakeClient, WaterIntakeClient>(client =>
    client.BaseAddress = new Uri(apiBaseUrl.TrimEnd('/') + '/'))
    .AddHttpMessageHandler<AuthHeaderHandler>();

await builder.Build().RunAsync();
