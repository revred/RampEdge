using FluentAssertions;
using Maker.RampEdge.Configuration;
using Maker.RampEdge.Http;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;

namespace Maker.RampEdge.Tests.Http;

public class StaticAppHeadersHandlerTests
{
    private readonly Mock<IOptions<RampEdgeSettings>> _settingsMock;
    private readonly StaticAppHeadersHandler _handler;
    private readonly HttpClient _httpClient;

    public StaticAppHeadersHandlerTests()
    {
        _settingsMock = new Mock<IOptions<RampEdgeSettings>>();
        _settingsMock.Setup(s => s.Value).Returns(new RampEdgeSettings
        {
            BusinessUnitKey = "test-business-unit-key"
        });

        _handler = new StaticAppHeadersHandler(_settingsMock.Object);

        var innerHandler = new TestHttpMessageHandler();
        _handler.InnerHandler = innerHandler;

        _httpClient = new HttpClient(_handler)
        {
            BaseAddress = new Uri("http://localhost:8080/")
        };
    }

    [Fact]
    public async Task Should_Add_BusinessUnitKey_Header()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/test");
        var response = await _httpClient.SendAsync(request);

        response.Should().NotBeNull();
        request.Headers.Should().ContainKey("BusinessUnitKey");
        request.Headers.GetValues("BusinessUnitKey").First().Should().Be("test-business-unit-key");
    }

    [Fact]
    public async Task Should_Not_Override_Existing_BusinessUnitKey_Header()
    {
        var customKey = "custom-business-unit-key";
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/test");
        request.Headers.Add("BusinessUnitKey", customKey);

        var response = await _httpClient.SendAsync(request);

        response.Should().NotBeNull();
        request.Headers.Should().ContainKey("BusinessUnitKey");
        request.Headers.GetValues("BusinessUnitKey").First().Should().Be(customKey);
    }

    [Fact]
    public async Task Should_Handle_Multiple_Requests()
    {
        var requests = new[]
        {
            new HttpRequestMessage(HttpMethod.Get, "/api/test1"),
            new HttpRequestMessage(HttpMethod.Post, "/api/test2"),
            new HttpRequestMessage(HttpMethod.Put, "/api/test3")
        };

        foreach (var request in requests)
        {
            var response = await _httpClient.SendAsync(request);

            response.Should().NotBeNull();
            request.Headers.Should().ContainKey("BusinessUnitKey");
            request.Headers.GetValues("BusinessUnitKey").First().Should().Be("test-business-unit-key");
        }
    }

    [Fact]
    public async Task Should_Work_With_Empty_BusinessUnitKey()
    {
        var settingsMock = new Mock<IOptions<RampEdgeSettings>>();
        settingsMock.Setup(s => s.Value).Returns(new RampEdgeSettings
        {
            BusinessUnitKey = string.Empty
        });

        var handler = new StaticAppHeadersHandler(settingsMock.Object);
        var innerHandler = new TestHttpMessageHandler();
        handler.InnerHandler = innerHandler;

        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:8080/")
        };

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/test");
        var response = await client.SendAsync(request);

        response.Should().NotBeNull();
        request.Headers.Should().ContainKey("BusinessUnitKey");
        request.Headers.GetValues("BusinessUnitKey").First().Should().BeEmpty();
    }

    private class TestHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                RequestMessage = request
            });
        }
    }
}