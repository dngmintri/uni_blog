using frontend.Services;

namespace frontend.Services;

public interface IUserUpdateService
{
    event Action<AuthResponse>? OnUserUpdated;
    void NotifyUserUpdated(AuthResponse user);
}

public class UserUpdateService : IUserUpdateService
{
    public event Action<AuthResponse>? OnUserUpdated;

    public void NotifyUserUpdated(AuthResponse user)
    {
        OnUserUpdated?.Invoke(user);
    }
}
