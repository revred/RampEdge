using FluentAssertions;
using Maker.RampEdge.Services;
using Maker.RampEdge.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace Maker.RampEdge.Tests.Services;

public class AuthenticationServiceTests : IDisposable
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<ITokenStorage> _tokenStorageMock;
    private readonly Mock<ILogger<AuthenticationService>> _loggerMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly AuthenticationService _authService;

    public AuthenticationServiceTests()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _tokenStorageMock = new Mock<ITokenStorage>();
        _loggerMock = new Mock<ILogger<AuthenticationService>>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://api.rampedge.com")
        };

        _httpClientFactoryMock
            .Setup(f => f.CreateClient("RampEdgeAuth"))
            .Returns(_httpClient);

        _authService = new AuthenticationService(
            _httpClientFactoryMock.Object,
            _tokenStorageMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnTrue()
    {
        // Arrange
        var email = "test@rampedge.com";
        var password = "TestPassword123!";
        var expectedResponse = new LoginResponse
        {
            Token = TestHelpers.GenerateMockToken(),
            RefreshToken = "refresh-token-123",
            IsRampEdgeUser = true,
            UserName = email
        };

        SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(expectedResponse));

        // Act
        var result = await _authService.LoginAsync(email, password);

        // Assert
        result.Should().BeTrue();
        _authService.IsAuthenticated.Should().BeTrue();
        _authService.UserName.Should().Be(email);
        _authService.IsRampEdgeUser.Should().BeTrue();

        // Verify tokens were stored
        _tokenStorageMock.Verify(
            s => s.SetAsync("rampedge_access_token", expectedResponse.Token),
            Times.Once);
        _tokenStorageMock.Verify(
            s => s.SetAsync("rampedge_refresh_token", expectedResponse.RefreshToken),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_ShouldReturnFalse()
    {
        // Arrange
        var email = "invalid@test.com";
        var password = "wrongpassword";

        SetupHttpResponse(HttpStatusCode.Unauthorized, "");

        // Act
        var result = await _authService.LoginAsync(email, password);

        // Assert
        result.Should().BeFalse();
        _authService.IsAuthenticated.Should().BeFalse();
        _authService.UserName.Should().BeNull();

        // Verify no tokens were stored
        _tokenStorageMock.Verify(
            s => s.SetAsync(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WithEmptyCredentials_ShouldReturnFalse()
    {
        // Act & Assert
        var resultEmptyEmail = await _authService.LoginAsync("", "password");
        var resultEmptyPassword = await _authService.LoginAsync("email", "");
        var resultBothEmpty = await _authService.LoginAsync("", "");

        resultEmptyEmail.Should().BeFalse();
        resultEmptyPassword.Should().BeFalse();
        resultBothEmpty.Should().BeFalse();
    }

    [Fact]
    public async Task LogoutAsync_ShouldClearAllTokensAndState()
    {
        // Arrange - First login to set up state
        await SetupAuthenticatedUser();

        // Act
        var result = await _authService.LogoutAsync();

        // Assert
        result.Should().BeTrue();
        _authService.IsAuthenticated.Should().BeFalse();
        _authService.UserName.Should().BeNull();
        _authService.User.Should().BeNull();
        _authService.AccessToken.Should().BeNull();
        _authService.RefreshToken.Should().BeNull();

        // Verify storage was cleared
        _tokenStorageMock.Verify(s => s.RemoveAsync("rampedge_access_token"), Times.Once);
        _tokenStorageMock.Verify(s => s.RemoveAsync("rampedge_refresh_token"), Times.Once);
        _tokenStorageMock.Verify(s => s.RemoveAsync("is_rampedge_user"), Times.Once);
    }

    [Fact]
    public async Task RefreshTokenAsync_WithValidToken_ShouldReturnTrue()
    {
        // Arrange
        var refreshToken = "valid-refresh-token";
        var newTokenResponse = new LoginResponse
        {
            Token = TestHelpers.GenerateMockToken(),
            RefreshToken = "new-refresh-token",
            IsRampEdgeUser = true
        };

        _tokenStorageMock.Setup(s => s.GetAsync("rampedge_refresh_token"))
            .ReturnsAsync(refreshToken);

        SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(newTokenResponse));

        // Act
        var result = await _authService.RefreshTokenAsync();

        // Assert
        result.Should().BeTrue();

        // Verify new tokens were stored
        _tokenStorageMock.Verify(
            s => s.SetAsync("rampedge_access_token", newTokenResponse.Token),
            Times.Once);
        _tokenStorageMock.Verify(
            s => s.SetAsync("rampedge_refresh_token", newTokenResponse.RefreshToken),
            Times.Once);
    }

    [Fact]
    public async Task RefreshTokenAsync_WithInvalidToken_ShouldReturnFalse()
    {
        // Arrange
        var refreshToken = "invalid-refresh-token";

        _tokenStorageMock.Setup(s => s.GetAsync("rampedge_refresh_token"))
            .ReturnsAsync(refreshToken);

        SetupHttpResponse(HttpStatusCode.Unauthorized, "");

        // Act
        var result = await _authService.RefreshTokenAsync();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task RefreshTokenAsync_WithNoRefreshToken_ShouldReturnFalse()
    {
        // Arrange
        _tokenStorageMock.Setup(s => s.GetAsync("rampedge_refresh_token"))
            .ReturnsAsync((string?)null);

        // Act
        var result = await _authService.RefreshTokenAsync();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetAccessTokenAsync_WithCachedToken_ShouldReturnToken()
    {
        // Arrange
        await SetupAuthenticatedUser();
        var expectedToken = TestHelpers.GenerateMockToken();
        _authService.GetType().GetProperty("AccessToken")?.SetValue(_authService, expectedToken);

        // Act
        var token = await _authService.GetAccessTokenAsync();

        // Assert
        token.Should().Be(expectedToken);
    }

    [Fact]
    public async Task GetAccessTokenAsync_WithStoredToken_ShouldReturnToken()
    {
        // Arrange
        var expectedToken = TestHelpers.GenerateMockToken();
        _tokenStorageMock.Setup(s => s.GetAsync("rampedge_access_token"))
            .ReturnsAsync(expectedToken);

        // Act
        var token = await _authService.GetAccessTokenAsync();

        // Assert
        token.Should().Be(expectedToken);
    }

    [Fact]
    public async Task GetAccessTokenAsync_WithNoToken_ShouldReturnNull()
    {
        // Arrange
        _tokenStorageMock.Setup(s => s.GetAsync("rampedge_access_token"))
            .ReturnsAsync((string?)null);

        // Act
        var token = await _authService.GetAccessTokenAsync();

        // Assert
        token.Should().BeNull();
    }

    [Fact]
    public async Task InitializeAsync_WithStoredTokens_ShouldRestoreState()
    {
        // Arrange
        var accessToken = TestHelpers.GenerateMockToken();
        var refreshToken = "stored-refresh-token";

        _tokenStorageMock.Setup(s => s.GetAsync("rampedge_access_token"))
            .ReturnsAsync(accessToken);
        _tokenStorageMock.Setup(s => s.GetAsync("rampedge_refresh_token"))
            .ReturnsAsync(refreshToken);
        _tokenStorageMock.Setup(s => s.GetAsync("is_rampedge_user"))
            .ReturnsAsync("true");

        // Act
        await _authService.InitializeAsync();

        // Assert
        _authService.IsAuthenticated.Should().BeTrue();
        _authService.IsRampEdgeUser.Should().BeTrue();
        _authService.UserName.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task OnChange_Event_ShouldFireOnStateChanges()
    {
        // Arrange
        var eventFired = false;
        _authService.OnChange += () => eventFired = true;

        var loginResponse = new LoginResponse
        {
            Token = TestHelpers.GenerateMockToken(),
            RefreshToken = "refresh-token",
            IsRampEdgeUser = true
        };

        SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(loginResponse));

        // Act
        await _authService.LoginAsync("test@example.com", "password");

        // Assert
        eventFired.Should().BeTrue();
    }

    [Fact]
    public async Task User_Property_ShouldContainClaimsFromToken()
    {
        // Arrange
        await SetupAuthenticatedUser();

        // Act & Assert
        _authService.User.Should().NotBeNull();
        _authService.User!.Identity.Should().NotBeNull();
        _authService.User.Identity!.IsAuthenticated.Should().BeTrue();
    }

    [Fact]
    public async Task GetAccessTokenExpiryAsync_WithValidToken_ShouldReturnExpiry()
    {
        // Arrange
        var token = TestHelpers.GenerateMockToken();
        _tokenStorageMock.Setup(s => s.GetAsync("rampedge_access_token"))
            .ReturnsAsync(token);

        // Act
        var expiry = await _authService.GetAccessTokenExpiryAsync();

        // Assert
        expiry.Should().NotBeNull();
        expiry.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task GetAccessTokenExpiryAsync_WithNoToken_ShouldReturnNull()
    {
        // Arrange
        _tokenStorageMock.Setup(s => s.GetAsync("rampedge_access_token"))
            .ReturnsAsync((string?)null);

        // Act
        var expiry = await _authService.GetAccessTokenExpiryAsync();

        // Assert
        expiry.Should().BeNull();
    }

    private async Task SetupAuthenticatedUser()
    {
        var loginResponse = new LoginResponse
        {
            Token = TestHelpers.GenerateMockToken(),
            RefreshToken = "refresh-token",
            IsRampEdgeUser = true,
            UserName = "test@rampedge.com"
        };

        SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(loginResponse));
        await _authService.LoginAsync("test@rampedge.com", "password");
    }

    private void SetupHttpResponse(HttpStatusCode statusCode, string content)
    {
        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
        _authService?.Dispose();
    }
}

// Extension for IDisposable support
public static class AuthenticationServiceExtensions
{
    public static void Dispose(this AuthenticationService service)
    {
        // Dispose any resources if needed
    }
}