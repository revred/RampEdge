using Maker.RampEdge;
using Maker.RampEdge.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Maker.RampEdge.Services;

public class AuthenticationService : IAuthenticationService
{
    private const string AccessTokenKey = "maker_access_token";
    private const string RefreshTokenKey = "maker_refresh_token";
    private const string IsMakerUserKey = "is_maker_user";
    private static readonly TimeSpan ExpirySkew = TimeSpan.FromSeconds(75);

    public bool IsAuthenticated { get; private set; }
    public bool? IsMakerAIUser { get; private set; }
    public string? UserName { get; private set; }
    public string? AccessToken { get; private set; }
    public string? RefreshToken { get; private set; }

    public event Action? OnChange;

    private readonly IMakerClient _makerClient;
    private readonly ITokenStorage _storage;
    private readonly ILogger<AuthenticationService> _logger;

    private readonly SemaphoreSlim _refreshGate = new(1, 1);
    private bool _stateHydratedOnce;

    public AuthenticationService(
        [FromKeyedServices("Auth")] IMakerClient authClient,
        ITokenStorage storage,
        ILogger<AuthenticationService> logger)
    {
        _makerClient = authClient;
        _storage = storage;
        _logger = logger;
    }

    public ClaimsPrincipal? User { get; private set; }
    public async Task StoreTokensAsync(LoginResponse loginResponse)
    {
        AccessToken = loginResponse.Token;
        RefreshToken = loginResponse.RefreshToken;

        // Save tokens
        await _storage.SetAsync(AccessTokenKey, loginResponse.Token);
        await _storage.SetAsync(RefreshTokenKey, loginResponse.RefreshToken);
        await _storage.SetAsync(IsMakerUserKey, loginResponse.IsMakerAI.ToString());

        await UpdateStateAsync(forceUpdate: true);
    }
    // Call once on app start to hydrate state from storage.
    public async Task InitializeAsync()
    {
        await UpdateStateAsync(forceUpdate: true);
    }
    public async Task<bool> LoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return false;

        try
        {
            var emailAndPassword = $"{email}|{password}";
            var result = await _makerClient.LoginAsync(body : new LoginRequest { EmailAndPassword = emailAndPassword });

            if (!string.IsNullOrWhiteSpace(result?.Token))
            {
                await StoreTokensAsync(result);
                return true;
            }

            _logger.LogWarning("Login returned empty token.");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during login.");
            return false;
        }
    }

    public async Task<bool> LogoutAsync()
    {
        try
        {
            await _storage.RemoveAsync(AccessTokenKey);
            await _storage.RemoveAsync(RefreshTokenKey);
            await _storage.RemoveAsync(IsMakerUserKey);

            AccessToken = RefreshToken = null;
            IsAuthenticated = false;
            UserName = null;
            User = null;

            NotifyStateChanged();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during logout.");
            return false;
        }
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        // Keep in-memory fast path
        if (!string.IsNullOrEmpty(AccessToken)) return AccessToken;

        var token = await _storage.GetAsync(AccessTokenKey);
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogDebug("No access token found.");
            return null;
        }

        AccessToken = token;
        return token;
    }

    public async Task<bool> RefreshTokenAsync(Action? onAuthenticationFailedCallback = null)
    {
        await _refreshGate.WaitAsync();
        try
        {
            // Skip if token is still valid (with skew)
            var exp = await GetAccessTokenExpiryAsync();
            if (exp.HasValue && exp.Value > DateTime.UtcNow.Add(ExpirySkew))
            {
                _logger.LogDebug("Token still valid; skipping refresh.");
                await UpdateStateAsync();
                return true;
            }

            var refreshToken = await _storage.GetAsync(RefreshTokenKey);
            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogInformation("Refresh token missing.");
                return false;
            }

            var tokenResponse = await _makerClient.RefreshTokenAsync(body : new RefreshTokenRequest
            {
                RefreshToken = refreshToken
            });

            if (tokenResponse is not null && !string.IsNullOrWhiteSpace(tokenResponse.Token))
            {
                await StoreTokensAsync(tokenResponse);
                _logger.LogInformation("Token refreshed successfully.");
                return true;
            }

            _logger.LogWarning("Token refresh failed: empty response or token.");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during token refresh. Logging out.");
            await LogoutAsync();
            onAuthenticationFailedCallback?.Invoke();
            return false;
        }
        finally
        {
            _refreshGate.Release();
        }
    }

    public async Task<DateTime?> GetAccessTokenExpiryAsync()
    {
        var token = AccessToken ?? await _storage.GetAsync(AccessTokenKey);
        if (string.IsNullOrWhiteSpace(token)) return null;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt.ValidTo; // UTC
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse access token expiry.");
            return null;
        }
    }

    private async Task UpdateStateAsync(bool forceUpdate = false)
    {
        if (_stateHydratedOnce && !forceUpdate)
        {
            _logger.LogDebug("State already hydrated; skipping.");
            return;
        }

        AccessToken = await _storage.GetAsync(AccessTokenKey);
        RefreshToken = await _storage.GetAsync(RefreshTokenKey);

        var isMakerStr = await _storage.GetAsync(IsMakerUserKey);
        IsMakerAIUser = bool.TryParse(isMakerStr, out var b) ? b : (bool?)null;

        if (!string.IsNullOrEmpty(AccessToken))
        {
            IsAuthenticated = true;
            UserName = GetEmailFromToken(AccessToken);
            User = GetClaimsPrincipal(AccessToken);
        }
        else
        {
            IsAuthenticated = false;
            UserName = null;
            User = null;
        }

        _stateHydratedOnce = true;
        NotifyStateChanged();
    }

    private ClaimsPrincipal GetClaimsPrincipal(string accessToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(accessToken);
        var identity = new ClaimsIdentity(jwt.Claims, "jwt");
        return new ClaimsPrincipal(identity);
    }

    private static string? GetEmailFromToken(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        return token.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "User";
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
