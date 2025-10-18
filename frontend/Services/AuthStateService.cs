using frontend.Models;
using System.Text.Json;

namespace frontend.Services;

public class AuthStateService
{
    private const string AUTH_KEY = "auth_user";
    private readonly LocalStorageService _localStorage;
    
    public bool IsAuthenticated { get; private set; }
    public AuthResponse? CurrentUser { get; private set; }

    public event Action? OnAuthStateChanged;

    public AuthStateService(LocalStorageService localStorage)
    {
        _localStorage = localStorage;
        _ = LoadFromStorageAsync();
    }

    public async Task SetUserAsync(AuthResponse user)
    {
        CurrentUser = user;
        IsAuthenticated = true;
        await SaveToStorageAsync();
        OnAuthStateChanged?.Invoke();
    }

    public async Task ClearUserAsync()
    {
        CurrentUser = null;
        IsAuthenticated = false;
        await ClearStorageAsync();
        OnAuthStateChanged?.Invoke();
    }

    private async Task LoadFromStorageAsync()
    {
        try
        {
            var json = await _localStorage.GetItemAsync(AUTH_KEY);
            if (!string.IsNullOrEmpty(json))
            {
                var user = JsonSerializer.Deserialize<AuthResponse>(json);
                if (user != null)
                {
                    CurrentUser = user;
                    IsAuthenticated = true;
                    OnAuthStateChanged?.Invoke();
                }
            }
        }
        catch
        {
            // Ignore errors when loading from storage
        }
    }

    private async Task SaveToStorageAsync()
    {
        try
        {
            if (CurrentUser != null)
            {
                var json = JsonSerializer.Serialize(CurrentUser);
                await _localStorage.SetItemAsync(AUTH_KEY, json);
            }
        }
        catch
        {
            // Ignore errors when saving to storage
        }
    }

    private async Task ClearStorageAsync()
    {
        try
        {
            await _localStorage.RemoveItemAsync(AUTH_KEY);
        }
        catch
        {
            // Ignore errors when clearing storage
        }
    }
}
