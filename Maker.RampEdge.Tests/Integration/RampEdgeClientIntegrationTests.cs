using FluentAssertions;
using Maker.RampEdge.Configuration;
using Maker.RampEdge.Extensions;
using Maker.RampEdge.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Moq;
using System.Net;
using System.Text.Json;

namespace Maker.RampEdge.Tests.Integration;

public class RampEdgeClientIntegrationTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly Mock<IJSRuntime> _jsRuntimeMock;
    private readonly TestHttpMessageHandler _httpMessageHandler;

    public RampEdgeClientIntegrationTests()
    {
        _jsRuntimeMock = new Mock<IJSRuntime>();
        _httpMessageHandler = new TestHttpMessageHandler();

        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(builder => builder.AddConsole());

        // Add JSRuntime mock
        services.AddSingleton(_jsRuntimeMock.Object);

        // Configure RampEdge settings
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["RampEdge:BaseAddress"] = "https://api.rampedge.com",
                ["RampEdge:BusinessUnitKey"] = "test-business-unit"
            })
            .Build();

        // Add RampEdge client
        services.AddRampEdgeClient(configuration,
            configurePublicClient: builder => builder.ConfigurePrimaryHttpMessageHandler(() => _httpMessageHandler),
            configureSecureClient: builder => builder.ConfigurePrimaryHttpMessageHandler(() => _httpMessageHandler));

        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public void Should_Register_All_Required_Services()
    {
        // Assert
        _serviceProvider.GetService<ITokenStorage>().Should().NotBeNull();
        _serviceProvider.GetService<IAuthenticationService>().Should().NotBeNull();
        _serviceProvider.GetService<IHttpClientFactory>().Should().NotBeNull();
    }

    [Fact]
    public void Should_Create_Named_HttpClients()
    {
        // Arrange
        var httpClientFactory = _serviceProvider.GetRequiredService<IHttpClientFactory>();

        // Act
        var authClient = httpClientFactory.CreateClient("RampEdgeAuth");
        var apiClient = httpClientFactory.CreateClient("RampEdgeApi");

        // Assert
        authClient.Should().NotBeNull();
        authClient.BaseAddress.Should().Be("https://api.rampedge.com/");

        apiClient.Should().NotBeNull();
        apiClient.BaseAddress.Should().Be("https://api.rampedge.com/");
    }

    [Fact]
    public async Task Should_Perform_Complete_Authentication_Flow()
    {
        // Arrange
        var authService = _serviceProvider.GetRequiredService<IAuthenticationService>();
        var token = TestHelpers.GenerateMockToken();
        var refreshToken = "refresh-token-123";

        var loginResponse = new
        {
            token = token,
            refreshToken = refreshToken,
            isRampEdgeUser = true,
            userName = "test@rampedge.com"
        };

        _httpMessageHandler.SetResponse(HttpStatusCode.OK, JsonSerializer.Serialize(loginResponse));

        _jsRuntimeMock.Setup(js => js.InvokeVoidAsync("localStorage.setItem", It.IsAny<string>(), It.IsAny<string>()))
            .Returns(ValueTask.CompletedTask);

        // Act
        var loginResult = await authService.LoginAsync("test@rampedge.com", "password");

        // Assert
        loginResult.Should().BeTrue();
        authService.IsAuthenticated.Should().BeTrue();
        authService.UserName.Should().Be("test@rampedge.com");
        authService.IsRampEdgeUser.Should().BeTrue();

        // Verify tokens were stored
        _jsRuntimeMock.Verify(
            js => js.InvokeVoidAsync("localStorage.setItem", "rampedge_access_token", token),
            Times.Once);
        _jsRuntimeMock.Verify(
            js => js.InvokeVoidAsync("localStorage.setItem", "rampedge_refresh_token", refreshToken),
            Times.Once);
    }

    [Fact]
    public async Task Should_Handle_Token_Refresh_Flow()
    {
        // Arrange
        var authService = _serviceProvider.GetRequiredService<IAuthenticationService>();
        var httpClientFactory = _serviceProvider.GetRequiredService<IHttpClientFactory>();
        var apiClient = httpClientFactory.CreateClient("RampEdgeApi");

        var oldToken = TestHelpers.GenerateMockToken(DateTime.UtcNow.AddMinutes(-5)); // Expired
        var newToken = TestHelpers.GenerateMockToken();
        var refreshToken = "refresh-token-123";

        // Setup stored tokens
        _jsRuntimeMock.Setup(js => js.InvokeAsync<string>("localStorage.getItem", "rampedge_access_token"))
            .ReturnsAsync(oldToken);
        _jsRuntimeMock.Setup(js => js.InvokeAsync<string>("localStorage.getItem", "rampedge_refresh_token"))
            .ReturnsAsync(refreshToken);
        _jsRuntimeMock.Setup(js => js.InvokeAsync<string>("localStorage.getItem", "is_rampedge_user"))
            .ReturnsAsync("true");

        var refreshResponse = new
        {
            token = newToken,
            refreshToken = "new-refresh-token",
            isRampEdgeUser = true
        };

        _httpMessageHandler.SetResponseSequence(new[]
        {
            new TestHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(refreshResponse)), // Refresh response
            new TestHttpResponse(HttpStatusCode.OK, "{ \"data\": \"success\" }") // API response
        });

        _jsRuntimeMock.Setup(js => js.InvokeVoidAsync("localStorage.setItem", It.IsAny<string>(), It.IsAny<string>()))
            .Returns(ValueTask.CompletedTask);

        // Initialize authentication service to load stored tokens
        await authService.InitializeAsync();

        // Act - Make API call that should trigger token refresh
        var response = await apiClient.GetAsync("/api/test");

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify new token was stored
        _jsRuntimeMock.Verify(
            js => js.InvokeVoidAsync("localStorage.setItem", "rampedge_access_token", newToken),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task Should_Handle_Logout_Flow()
    {
        // Arrange
        var authService = _serviceProvider.GetRequiredService<IAuthenticationService>();

        // First login
        var token = TestHelpers.GenerateMockToken();
        var loginResponse = new
        {
            token = token,
            refreshToken = "refresh-token",
            isRampEdgeUser = true,
            userName = "test@rampedge.com"
        };

        _httpMessageHandler.SetResponse(HttpStatusCode.OK, JsonSerializer.Serialize(loginResponse));
        _jsRuntimeMock.Setup(js => js.InvokeVoidAsync("localStorage.setItem", It.IsAny<string>(), It.IsAny<string>()))
            .Returns(ValueTask.CompletedTask);
        _jsRuntimeMock.Setup(js => js.InvokeVoidAsync("localStorage.removeItem", It.IsAny<string>()))
            .Returns(ValueTask.CompletedTask);

        await authService.LoginAsync("test@rampedge.com", "password");

        // Act - Logout
        var logoutResult = await authService.LogoutAsync();

        // Assert
        logoutResult.Should().BeTrue();
        authService.IsAuthenticated.Should().BeFalse();
        authService.UserName.Should().BeNull();
        authService.User.Should().BeNull();

        // Verify tokens were removed
        _jsRuntimeMock.Verify(
            js => js.InvokeVoidAsync("localStorage.removeItem", "rampedge_access_token"),
            Times.Once);
        _jsRuntimeMock.Verify(
            js => js.InvokeVoidAsync("localStorage.removeItem", "rampedge_refresh_token"),
            Times.Once);
    }

    [Fact]
    public async Task Should_Handle_Authentication_State_Changes()
    {
        // Arrange
        var authService = _serviceProvider.GetRequiredService<IAuthenticationService>();
        var stateChangeCount = 0;

        authService.OnChange += () => stateChangeCount++;

        var token = TestHelpers.GenerateMockToken();
        var loginResponse = new
        {
            token = token,
            refreshToken = "refresh-token",
            isRampEdgeUser = true,
            userName = "test@rampedge.com"
        };

        _httpMessageHandler.SetResponse(HttpStatusCode.OK, JsonSerializer.Serialize(loginResponse));
        _jsRuntimeMock.Setup(js => js.InvokeVoidAsync("localStorage.setItem", It.IsAny<string>(), It.IsAny<string>()))
            .Returns(ValueTask.CompletedTask);
        _jsRuntimeMock.Setup(js => js.InvokeVoidAsync("localStorage.removeItem", It.IsAny<string>()))
            .Returns(ValueTask.CompletedTask);

        // Act
        await authService.LoginAsync("test@rampedge.com", "password"); // Should trigger state change
        await authService.LogoutAsync(); // Should trigger state change

        // Assert
        stateChangeCount.Should().BeGreaterOrEqualTo(2);
    }

    [Fact]
    public async Task Should_Add_Business_Unit_Header_To_Requests()
    {
        // Arrange
        var httpClientFactory = _serviceProvider.GetRequiredService<IHttpClientFactory>();
        var authClient = httpClientFactory.CreateClient("RampEdgeAuth");

        _httpMessageHandler.SetResponse(HttpStatusCode.OK, "{}");

        // Act
        var response = await authClient.GetAsync("/api/public/test");

        // Assert
        var lastRequest = _httpMessageHandler.LastRequest;
        lastRequest.Should().NotBeNull();
        lastRequest!.Headers.Should().ContainKey("X-Business-Unit-Key");
        lastRequest.Headers.GetValues("X-Business-Unit-Key").Should().Contain("test-business-unit");
    }

    [Fact]
    public void Should_Throw_When_BaseAddress_Missing()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["RampEdge:BusinessUnitKey"] = "test-key"
                // BaseAddress intentionally missing
            })
            .Build();

        // Act & Assert
        var action = () => services.AddRampEdgeClient(configuration);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*BaseAddress*missing*");
    }

    [Fact]
    public void Should_Support_Programmatic_Configuration()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(_jsRuntimeMock.Object);

        // Act
        services.AddRampEdgeClient(settings =>
        {
            settings.BaseAddress = "https://custom.api.com";
            settings.BusinessUnitKey = "custom-key";
        });

        var serviceProvider = services.BuildServiceProvider();
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var client = httpClientFactory.CreateClient("RampEdgeAuth");

        // Assert
        client.BaseAddress.Should().Be("https://custom.api.com/");

        serviceProvider.Dispose();
    }

    public void Dispose()
    {
        _serviceProvider?.Dispose();
        _httpMessageHandler?.Dispose();
    }

    private class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly Queue<TestHttpResponse> _responses = new();
        private TestHttpResponse? _defaultResponse;

        public HttpRequestMessage? LastRequest { get; private set; }

        public void SetResponse(HttpStatusCode statusCode, string content)
        {
            _defaultResponse = new TestHttpResponse(statusCode, content);
        }

        public void SetResponseSequence(TestHttpResponse[] responses)
        {
            _responses.Clear();
            foreach (var response in responses)
            {
                _responses.Enqueue(response);
            }
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;

            var response = _responses.Count > 0 ? _responses.Dequeue() : _defaultResponse;

            return Task.FromResult(new HttpResponseMessage(response?.StatusCode ?? HttpStatusCode.OK)
            {
                Content = new StringContent(response?.Content ?? "{}"),
                RequestMessage = request
            });
        }
    }

    private record TestHttpResponse(HttpStatusCode StatusCode, string Content);
}