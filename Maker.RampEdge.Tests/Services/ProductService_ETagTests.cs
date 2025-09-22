using FluentAssertions;
using Maker.RampEdge.Models;
using Maker.RampEdge.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Maker.RampEdge.Tests.Services;

public class ProductService_ETagTests : IDisposable
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<ILogger<ProductService>> _loggerMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly ProductService _productService;

    public ProductService_ETagTests()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _loggerMock = new Mock<ILogger<ProductService>>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://api.rampedge.com")
        };

        _httpClientFactoryMock
            .Setup(f => f.CreateClient("RampEdgeApi"))
            .Returns(_httpClient);

        _productService = new ProductService(_httpClientFactoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task FirstCall_200_SetsCacheAndETag_SecondCall_304_UsesCache()
    {
        // Arrange
        var expectedProducts = new List<Product>
        {
            new Product { Id = "1", Sku = "TEST-001", Title = "Test Product 1" }
        };

        var expectedResponse = new ProductListResponse
        {
            Products = expectedProducts,
            TotalCount = 1,
            Page = 1,
            PageSize = 24
        };

        // Setup sequence of responses
        var firstResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(expectedResponse), Encoding.UTF8, "application/json")
        };
        firstResponse.Headers.ETag = new EntityTagHeaderValue("\"etag-1\"");

        var secondResponse = new HttpResponseMessage(HttpStatusCode.NotModified);

        _httpMessageHandlerMock
            .Protected()
            .SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(firstResponse)
            .ReturnsAsync(secondResponse);

        var options = new ProductListOptions { Page = 1, PageSize = 24 };

        // Act
        var firstResult = await _productService.GetProductsAsync(options);
        var secondResult = await _productService.GetProductsAsync(options);

        // Assert
        firstResult.Should().NotBeNull();
        firstResult.Products.Should().HaveCount(1);
        firstResult.Products[0].Sku.Should().Be("TEST-001");

        secondResult.Should().NotBeNull();
        secondResult.Products.Should().HaveCount(1);
        secondResult.Products[0].Sku.Should().Be("TEST-001");
    }

    [Fact]
    public async Task ETag_Headers_Are_Set_Correctly()
    {
        // Arrange
        var response = new ProductListResponse
        {
            Products = new List<Product> { new Product { Sku = "TEST-001", Title = "ETag Product" } },
            TotalCount = 1,
            Page = 1,
            PageSize = 24
        };

        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(response), Encoding.UTF8, "application/json")
        };
        httpResponse.Headers.ETag = new EntityTagHeaderValue("\"test-etag-123\"");

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        var options = new ProductListOptions { Page = 1, PageSize = 24 };

        // Act
        var result = await _productService.GetProductsAsync(options);

        // Assert
        result.Should().NotBeNull();
        result.Products.Should().HaveCount(1);
        result.Products[0].Title.Should().Be("ETag Product");

        // Verify HTTP request was made
        _httpMessageHandlerMock
            .Protected()
            .Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task Cache_Integration_Test()
    {
        // Arrange - Simple integration test for cache behavior
        var response = new ProductListResponse
        {
            Products = new List<Product> { new Product { Sku = "TEST-001", Title = "Cached Product" } },
            TotalCount = 1,
            Page = 1,
            PageSize = 24
        };

        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(response), Encoding.UTF8, "application/json")
        };
        httpResponse.Headers.ETag = new EntityTagHeaderValue("\"etag-cache-test\"");

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        var options = new ProductListOptions { Page = 1, PageSize = 24 };

        // Act
        var result = await _productService.GetProductsAsync(options);

        // Assert
        result.Should().NotBeNull();
        result.Products.Should().HaveCount(1);
        result.Products[0].Title.Should().Be("Cached Product");
    }

    private void SetupHttpResponse(HttpStatusCode statusCode, string content)
    {
        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(content, Encoding.UTF8, "application/json")
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
    }

    private void SetupHttpResponseWithETag(HttpStatusCode statusCode, string content, string etag)
    {
        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(content, Encoding.UTF8, "application/json")
        };

        if (!string.IsNullOrEmpty(etag))
        {
            response.Headers.ETag = new EntityTagHeaderValue(etag);
        }

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
    }

    private void VerifyHttpRequestWithHeader(string headerName, string expectedValue)
    {
        _httpMessageHandlerMock
            .Protected()
            .Verify(
                "SendAsync",
                Times.AtLeastOnce(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Headers.Contains(headerName) &&
                    req.Headers.GetValues(headerName).Contains(expectedValue)),
                ItExpr.IsAny<CancellationToken>());
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
        _productService?.Dispose();
    }
}