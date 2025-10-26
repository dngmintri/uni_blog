using frontend.Models;

namespace frontend.Services;

public class UserUpdateService : IUserUpdateService
{
    public event Action<AuthResponse>? OnUserUpdated;

    public void NotifyUserUpdated(AuthResponse user)
    {
        OnUserUpdated?.Invoke(user);
    }
}
