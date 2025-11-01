using frontend.Models;
namespace frontend.Services;

public interface IUserUpdateService
{
    event Action<AuthResponse>? OnUserUpdated;
    void NotifyUserUpdated(AuthResponse user);
}