using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using frontend;
using frontend.Services;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5081/") });
builder.Services.AddMudServices();
builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<AuthStateService>();
builder.Services.AddScoped<AuthService>();

await builder.Build().RunAsync();
