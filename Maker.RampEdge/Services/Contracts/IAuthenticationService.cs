using System.Security.Claims;

namespace Maker.RampEdge.Services.Contracts;

public interface IAuthenticationService
{
    ClaimsPrincipal? User { get; }
    bool IsAuthenticated { get; }
    string? AccessToken { get; }
    string? RefreshToken { get; }
    bool? IsRampEdgeUser { get; }
    string? UserName { get; }
    event Action? OnChange;

    Task InitializeAsync();
    Task<bool> LoginAsync(string email, string password);
    Task<bool> LogoutAsync();
    Task<bool> RefreshTokenAsync(Action? onAuthenticationFailedCallback = null);
    Task<string?> GetAccessTokenAsync();
    Task<DateTime?> GetAccessTokenExpiryAsync();
}