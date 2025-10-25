using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Blazored.LocalStorage;
using frontend.Services;
using frontend;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Cấu hình HttpClient để gọi backend API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5000") });

// Đăng ký LocalStorage Service
builder.Services.AddBlazoredLocalStorage();

// Đăng ký AuthStateProvider
builder.Services.AddScoped<AuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<AuthStateProvider>());

// Đăng ký TokenManagerService
builder.Services.AddScoped<ITokenManagerService, TokenManagerService>();

// Đăng ký AuthService
builder.Services.AddScoped<IAuthService, AuthService>();

// Đăng ký UserService
builder.Services.AddScoped<IUserService, UserService>();

// Đăng ký AdminService
builder.Services.AddScoped<IAdminService, AdminService>();

// Đăng ký PageTitleService
builder.Services.AddScoped<PageTitleService>();

// Đăng ký UserUpdateService
builder.Services.AddScoped<IUserUpdateService, UserUpdateService>();

// Đăng ký Authorization
builder.Services.AddAuthorizationCore();

//Đăng ký PostService
builder.Services.AddScoped<IPostService, PostService>();

//Đăng ký UploadService
builder.Services.AddScoped<IUploadService, UploadService>();

//Đăng ký CommentService
builder.Services.AddScoped<CommentService>();

// Cấu hình Blazorise
builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons();

await builder.Build().RunAsync();
