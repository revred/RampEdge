using FluentAssertions;
using Maker.RampEdge.Models;
using Maker.RampEdge.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Linq.Expressions;
using System.Net;
using System.Text.Json;

namespace Maker.RampEdge.Tests.Services;

public class ProductServiceTests : IDisposable
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<ILogger<ProductService>> _loggerMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly ProductService _productService;

    public ProductServiceTests()
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
    public async Task GetProductsAsync_WithValidResponse_ShouldReturnProducts()
    {
        // Arrange
        var expectedProducts = new List<Product>
        {
            new Product
            {
                Id = "1",
                Sku = "TEST-001",
                Title = "Test Product 1",
                Price = 99.99m,
                Currency = "USD"
            },
            new Product
            {
                Id = "2",
                Sku = "TEST-002",
                Title = "Test Product 2",
                Price = 149.99m,
                Currency = "USD"
            }
        };

        var expectedResponse = new ProductListResponse
        {
            Products = expectedProducts,
            TotalCount = 2,
            Page = 1,
            PageSize = 24,
            HasNextPage = false,
            HasPreviousPage = false
        };

        SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(expectedResponse));

        var options = new ProductListOptions
        {
            Page = 1,
            PageSize = 24
        };

        // Act
        var result = await _productService.GetProductsAsync(options);

        // Assert
        result.Should().NotBeNull();
        result.Products.Should().HaveCount(2);
        result.Products[0].Sku.Should().Be("TEST-001");
        result.Products[1].Sku.Should().Be("TEST-002");
        result.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task GetProductsAsync_WithCategory_ShouldIncludeCategoryInQuery()
    {
        // Arrange
        var expectedResponse = new ProductListResponse
        {
            Products = new List<Product>(),
            TotalCount = 0
        };

        SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(expectedResponse));

        var options = new ProductListOptions
        {
            Category = "valves",
            Page = 1,
            PageSize = 12
        };

        // Act
        await _productService.GetProductsAsync(options);

        // Assert
        VerifyHttpRequest(req =>
            req.RequestUri!.Query.Contains("category=valves") &&
            req.RequestUri.Query.Contains("page=1") &&
            req.RequestUri.Query.Contains("pageSize=12"));
    }

    [Fact]
    public async Task GetProductAsync_WithValidSku_ShouldReturnProduct()
    {
        // Arrange
        var expectedProduct = new Product
        {
            Id = "1",
            Sku = "TEST-001",
            Title = "Test Product",
            Price = 99.99m,
            Currency = "USD",
            Specifications = new List<ProductSpecification>
            {
                new ProductSpecification { Key = "Material", Value = "Steel", Unit = "" }
            }
        };

        SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(expectedProduct));

        // Act
        var result = await _productService.GetProductAsync("TEST-001");

        // Assert
        result.Should().NotBeNull();
        result!.Sku.Should().Be("TEST-001");
        result.Title.Should().Be("Test Product");
        result.Specifications.Should().HaveCount(1);
        result.Specifications[0].Key.Should().Be("Material");
    }

    [Fact]
    public async Task GetProductAsync_WithNonExistentSku_ShouldReturnNull()
    {
        // Arrange
        SetupHttpResponse(HttpStatusCode.NotFound, "");

        // Act
        var result = await _productService.GetProductAsync("NON-EXISTENT");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetProductChangesAsync_WithValidResponse_ShouldReturnChanges()
    {
        // Arrange
        var expectedChanges = new List<ProductChange>
        {
            new ProductChange
            {
                Id = "change-1",
                Sku = "TEST-001",
                Operation = "update",
                Timestamp = DateTime.UtcNow,
                Product = new Product { Sku = "TEST-001", Title = "Updated Product" }
            }
        };

        var expectedResponse = new ProductChangesResponse
        {
            Changes = expectedChanges,
            ETag = "new-etag-123",
            HasMore = false
        };

        SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(expectedResponse));

        // Act
        var result = await _productService.GetProductChangesAsync("old-etag");

        // Assert
        result.Should().NotBeNull();
        result.Changes.Should().HaveCount(1);
        result.Changes[0].Sku.Should().Be("TEST-001");
        result.Changes[0].Operation.Should().Be("update");
        result.ETag.Should().Be("new-etag-123");
    }

    [Fact]
    public async Task GetProductChangesAsync_ShouldFireOnProductsChangedEvent()
    {
        // Arrange
        var eventFired = false;
        List<ProductChange>? receivedChanges = null;

        _productService.OnProductsChanged += (changes) =>
        {
            eventFired = true;
            receivedChanges = changes;
        };

        var expectedChanges = new List<ProductChange>
        {
            new ProductChange
            {
                Sku = "TEST-001",
                Operation = "update",
                Product = new Product { Sku = "TEST-001", Title = "Updated Product" }
            }
        };

        var expectedResponse = new ProductChangesResponse
        {
            Changes = expectedChanges,
            ETag = "new-etag"
        };

        SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(expectedResponse));

        // Act
        await _productService.GetProductChangesAsync();

        // Assert
        eventFired.Should().BeTrue();
        receivedChanges.Should().HaveCount(1);
        receivedChanges![0].Sku.Should().Be("TEST-001");
    }

    [Fact]
    public async Task InvalidateCacheAsync_ShouldClearAllCaches()
    {
        // Arrange
        var productResponse = new Product { Sku = "TEST-001", Title = "Test Product" };
        SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(productResponse));

        // First, populate cache
        await _productService.GetProductAsync("TEST-001");

        // Reset mock to verify cache miss
        _httpMessageHandlerMock.Reset();
        SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(productResponse));

        // Act
        await _productService.InvalidateCacheAsync();
        await _productService.GetProductAsync("TEST-001");

        // Assert
        // Should make HTTP request again after cache invalidation
        VerifyHttpRequest(req => req.RequestUri!.PathAndQuery.Contains("TEST-001"));
    }

    [Fact]
    public async Task StartDeltaRefreshAsync_ShouldReturnTrue()
    {
        // Arrange
        var changesResponse = new ProductChangesResponse
        {
            Changes = new List<ProductChange>(),
            ETag = "initial-etag"
        };

        SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(changesResponse));

        // Act
        var result = await _productService.StartDeltaRefreshAsync(TimeSpan.FromSeconds(2));

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetProductsAsync_WithHttpError_ShouldFireErrorEvent()
    {
        // Arrange
        var errorFired = false;
        string? errorMessage = null;

        _productService.OnError += (error) =>
        {
            errorFired = true;
            errorMessage = error;
        };

        SetupHttpResponse(HttpStatusCode.InternalServerError, "Server Error");

        var options = new ProductListOptions();

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(async () =>
            await _productService.GetProductsAsync(options));

        errorFired.Should().BeTrue();
        errorMessage.Should().Contain("Failed to fetch products");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task GetProductAsync_WithInvalidSku_ShouldHandleGracefully(string? sku)
    {
        // Arrange
        SetupHttpResponse(HttpStatusCode.NotFound, "");

        // Act
        var result = await _productService.GetProductAsync(sku!);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetProductsAsync_WithSearchTerm_ShouldIncludeSearchInQuery()
    {
        // Arrange
        var expectedResponse = new ProductListResponse { Products = new List<Product>() };
        SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(expectedResponse));

        var options = new ProductListOptions
        {
            Search = "valve connector",
            Page = 1,
            PageSize = 24
        };

        // Act
        await _productService.GetProductsAsync(options);

        // Assert
        VerifyHttpRequest(req => req.RequestUri!.Query.Contains("search=valve%20connector"));
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

    private void VerifyHttpRequest(Expression<Func<HttpRequestMessage, bool>> match)
    {
        _httpMessageHandlerMock
            .Protected()
            .Verify(
                "SendAsync",
                Times.AtLeastOnce(),
                ItExpr.Is(match),
                ItExpr.IsAny<CancellationToken>());
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
        _productService?.Dispose();
    }
}