using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Moq;
using Moq.Protected;

namespace Maker.RampEdge.Tests;

public static class TestHelpers
{
    public static HttpClient CreateMockHttpClient(HttpResponseMessage response)
    {
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response)
            .Verifiable();

        return new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost:8080/")
        };
    }

    public static HttpClient CreateMockHttpClient(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
    {
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage request, CancellationToken _) => responseFactory(request))
            .Verifiable();

        return new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost:8080/")
        };
    }

    public static IHttpClientFactory CreateMockHttpClientFactory(string clientName, HttpClient httpClient)
    {
        var factoryMock = new Mock<IHttpClientFactory>();
        factoryMock
            .Setup(f => f.CreateClient(clientName))
            .Returns(httpClient);
        return factoryMock.Object;
    }

    public static StringContent CreateJsonContent(string json)
    {
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    public static string GenerateMockToken()
    {
        return "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IlRlc3QgVXNlciIsImlhdCI6MTUxNjIzOTAyMiwiZXhwIjoxOTk5OTk5OTk5fQ.hqWGX4p5HS5nRJmQdWOs-kVq9_tL7rQkUPAJ7RGX5eI";
    }

    public static class JsonResponses
    {
        public static string LoginSuccess = @"{
            ""access_token"": """ + GenerateMockToken() + @""",
            ""refresh_token"": ""mock-refresh-token"",
            ""expires_in"": 3600,
            ""token_type"": ""Bearer""
        }";

        public static string LoginFailure = @"{
            ""error"": ""invalid_credentials"",
            ""error_description"": ""Invalid username or password""
        }";

        public static string RefreshTokenSuccess = @"{
            ""access_token"": """ + GenerateMockToken() + @""",
            ""refresh_token"": ""new-refresh-token"",
            ""expires_in"": 3600,
            ""token_type"": ""Bearer""
        }";

        public static string UserInfo = @"{
            ""id"": ""user123"",
            ""email"": ""test@rampedge.com"",
            ""name"": ""Test User"",
            ""isRampEdgeUser"": true
        }";
    }
}