using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;
using Maker.RampEdge.Http;
using Maker.RampEdge.Services.Contracts;
using Moq;

namespace Maker.RampEdge.Tests.Http;

public class BearerTokenHandlerTests
{
    private readonly Mock<IAuthenticationService> _authServiceMock;
    private readonly BearerTokenHandler _handler;
    private readonly HttpClient _httpClient;

    public BearerTokenHandlerTests()
    {
        _authServiceMock = new Mock<IAuthenticationService>();
        _handler = new BearerTokenHandler(_authServiceMock.Object);

        var innerHandler = new TestHttpMessageHandler();
        _handler.InnerHandler = innerHandler;

        _httpClient = new HttpClient(_handler)
        {
            BaseAddress = new Uri("http://localhost:8080/")
        };
    }

    [Fact]
    public async Task Should_Add_Bearer_Token_To_Request()
    {
        var token = "test-access-token";
        var expiry = DateTime.UtcNow.AddHours(1);

        _authServiceMock.Setup(a => a.GetAccessTokenExpiryAsync())
            .ReturnsAsync(expiry);
        _authServiceMock.Setup(a => a.GetAccessTokenAsync())
            .ReturnsAsync(token);

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/test");
        var response = await _httpClient.SendAsync(request);

        response.Should().NotBeNull();
        request.Headers.Authorization.Should().NotBeNull();
        request.Headers.Authorization!.Scheme.Should().Be("Bearer");
        request.Headers.Authorization.Parameter.Should().Be(token);
    }

    [Fact]
    public async Task Should_Skip_Token_For_Public_Endpoints()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/public/test");
        var response = await _httpClient.SendAsync(request);

        response.Should().NotBeNull();
        request.Headers.Authorization.Should().BeNull();

        _authServiceMock.Verify(a => a.GetAccessTokenExpiryAsync(), Times.Never);
        _authServiceMock.Verify(a => a.GetAccessTokenAsync(), Times.Never);
    }

    [Fact]
    public async Task Should_Refresh_Token_When_Expired()
    {
        var newToken = "new-token";
        var expiredTime = DateTime.UtcNow.AddMinutes(-1);
        var newExpiry = DateTime.UtcNow.AddHours(1);

        _authServiceMock.SetupSequence(a => a.GetAccessTokenExpiryAsync())
            .ReturnsAsync(expiredTime)
            .ReturnsAsync(newExpiry);

        _authServiceMock.Setup(a => a.RefreshTokenAsync(null))
            .ReturnsAsync(true);

        _authServiceMock.Setup(a => a.GetAccessTokenAsync())
            .ReturnsAsync(newToken);

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/test");
        var response = await _httpClient.SendAsync(request);

        response.Should().NotBeNull();
        request.Headers.Authorization!.Parameter.Should().Be(newToken);

        _authServiceMock.Verify(a => a.RefreshTokenAsync(null), Times.Once);
    }

    [Fact]
    public async Task Should_Return_Unauthorized_When_Refresh_Fails()
    {
        var expiredTime = DateTime.UtcNow.AddMinutes(-1);

        _authServiceMock.Setup(a => a.GetAccessTokenExpiryAsync())
            .ReturnsAsync(expiredTime);

        _authServiceMock.Setup(a => a.RefreshTokenAsync(null))
            .ReturnsAsync(false);

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/test");
        var response = await _httpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        _authServiceMock.Verify(a => a.RefreshTokenAsync(null), Times.Once);
    }

    [Fact]
    public async Task Should_Call_Unauthorized_Callback_On_401_Response()
    {
        var callbackInvoked = false;
        Func<HttpRequestMessage, Task> callback = async (request) =>
        {
            callbackInvoked = true;
            await Task.CompletedTask;
        };

        var handler = new BearerTokenHandler(_authServiceMock.Object, callback);
        var innerHandler = new TestHttpMessageHandler(HttpStatusCode.Unauthorized);
        handler.InnerHandler = innerHandler;

        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:8080/")
        };

        var token = "test-token";
        var expiry = DateTime.UtcNow.AddHours(1);

        _authServiceMock.Setup(a => a.GetAccessTokenExpiryAsync())
            .ReturnsAsync(expiry);
        _authServiceMock.Setup(a => a.GetAccessTokenAsync())
            .ReturnsAsync(token);

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/test");
        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        callbackInvoked.Should().BeTrue();
    }

    private class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;

        public TestHttpMessageHandler(HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            _statusCode = statusCode;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(_statusCode)
            {
                RequestMessage = request
            });
        }
    }
}