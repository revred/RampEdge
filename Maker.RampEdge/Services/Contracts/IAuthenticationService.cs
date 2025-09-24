using System.Security.Claims;

namespace Maker.RampEdge.Services.Contracts;

public interface IAuthenticationService
{
    ClaimsPrincipal? User { get; }
    bool IsAuthenticated { get; }
    public string? AccessToken { get; }
    public string? RefreshToken { get; }
    bool? IsMakerAIUser { get; }
    string? UserName { get; }
    event Action? OnChange;

    Task InitializeAsync(); 
    Task<bool> LoginAsync(string email, string password);
    Task<bool> LogoutAsync();

    Task<bool> RefreshTokenAsync(Action? onAuthenticationFailedCallback = null);
    Task<string?> GetAccessTokenAsync();
    Task<DateTime?> GetAccessTokenExpiryAsync();
}
