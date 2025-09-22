using Maker.RampEdge.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace Maker.RampEdge.Services;

public class AuthenticationService : IAuthenticationService
{
    private const string AccessTokenKey = "rampedge_access_token";
    private const string RefreshTokenKey = "rampedge_refresh_token";
    private const string IsRampEdgeUserKey = "is_rampedge_user";
    private static readonly TimeSpan ExpirySkew = TimeSpan.FromSeconds(75);

    public bool IsAuthenticated { get; private set; }
    public bool? IsRampEdgeUser { get; private set; }
    public string? UserName { get; private set; }
    public string? AccessToken { get; private set; }
    public string? RefreshToken { get; private set; }

    public event Action? OnChange;

    private readonly HttpClient _authClient;
    private readonly ITokenStorage _storage;
    private readonly ILogger<AuthenticationService> _logger;

    private readonly SemaphoreSlim _refreshGate = new(1, 1);
    private bool _stateHydratedOnce;

    public AuthenticationService(
        IHttpClientFactory httpClientFactory,
        ITokenStorage storage,
        ILogger<AuthenticationService> logger)
    {
        _authClient = httpClientFactory.CreateClient("RampEdgeAuth");
        _storage = storage;
        _logger = logger;
    }

    public ClaimsPrincipal? User { get; private set; }

    public async Task StoreTokensAsync(LoginResponse loginResponse)
    {
        AccessToken = loginResponse.Token;
        RefreshToken = loginResponse.RefreshToken;

        await _storage.SetAsync(AccessTokenKey, loginResponse.Token);
        await _storage.SetAsync(RefreshTokenKey, loginResponse.RefreshToken);
        await _storage.SetAsync(IsRampEdgeUserKey, loginResponse.IsRampEdgeUser.ToString());

        await UpdateStateAsync(forceUpdate: true);
    }

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
            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password
            };

            var json = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _authClient.PostAsync("/api/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (loginResponse != null && !string.IsNullOrWhiteSpace(loginResponse.Token))
                {
                    await StoreTokensAsync(loginResponse);
                    return true;
                }
            }

            _logger.LogWarning("Login failed with status: {StatusCode}", response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during login for email: {Email}", email);
            return false;
        }
    }

    public async Task<bool> LogoutAsync()
    {
        try
        {
            await _storage.RemoveAsync(AccessTokenKey);
            await _storage.RemoveAsync(RefreshTokenKey);
            await _storage.RemoveAsync(IsRampEdgeUserKey);

            AccessToken = RefreshToken = null;
            IsAuthenticated = false;
            UserName = null;
            User = null;
            IsRampEdgeUser = null;

            NotifyStateChanged();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during logout");
            return false;
        }
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        if (!string.IsNullOrEmpty(AccessToken)) return AccessToken;

        var token = await _storage.GetAsync(AccessTokenKey);
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogDebug("No access token found in storage");
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
            var exp = await GetAccessTokenExpiryAsync();
            if (exp.HasValue && exp.Value > DateTime.UtcNow.Add(ExpirySkew))
            {
                _logger.LogDebug("Token still valid; skipping refresh");
                await UpdateStateAsync();
                return true;
            }

            var refreshToken = await _storage.GetAsync(RefreshTokenKey);
            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogInformation("Refresh token missing");
                return false;
            }

            var refreshRequest = new RefreshTokenRequest
            {
                RefreshToken = refreshToken
            };

            var json = JsonSerializer.Serialize(refreshRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _authClient.PostAsync("/api/auth/refresh", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (tokenResponse != null && !string.IsNullOrWhiteSpace(tokenResponse.Token))
                {
                    await StoreTokensAsync(tokenResponse);
                    _logger.LogInformation("Token refreshed successfully");
                    return true;
                }
            }

            _logger.LogWarning("Token refresh failed with status: {StatusCode}", response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during token refresh. Logging out");
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
            return jwt.ValidTo;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse access token expiry");
            return null;
        }
    }

    private async Task UpdateStateAsync(bool forceUpdate = false)
    {
        if (_stateHydratedOnce && !forceUpdate)
        {
            _logger.LogDebug("State already hydrated; skipping");
            return;
        }

        AccessToken = await _storage.GetAsync(AccessTokenKey);
        RefreshToken = await _storage.GetAsync(RefreshTokenKey);

        var isRampEdgeStr = await _storage.GetAsync(IsRampEdgeUserKey);
        IsRampEdgeUser = bool.TryParse(isRampEdgeStr, out var b) ? b : (bool?)null;

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
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(accessToken);
            var identity = new ClaimsIdentity(jwt.Claims, "jwt");
            return new ClaimsPrincipal(identity);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse JWT claims");
            return new ClaimsPrincipal();
        }
    }

    private static string? GetEmailFromToken(string jwt)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            return token.Claims.FirstOrDefault(c => c.Type == "email" || c.Type == ClaimTypes.Email)?.Value ?? "User";
        }
        catch
        {
            return "User";
        }
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}

// Data models for API communication
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public bool IsRampEdgeUser { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string UserName { get; set; } = string.Empty;
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}