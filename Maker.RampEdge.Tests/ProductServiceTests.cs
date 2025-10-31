using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Maker.RampEdge.Services;
using Maker.RampEdge.Services.Contracts;
using Maker.RampEdge.Tests.Helper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Moq;
using Xunit;

namespace Maker.RampEdge.Tests
{
    public class ProductServiceTests
    {
        ILogger<ProductService> logger = NullLogger<ProductService>.Instance;
        [Fact]
        public async Task HandleAsync_WhenCalled_ReturnsProductGroups()
        {
            // Arrange
            var expectedResponse = new ProductListResponse
            {
                Products = new List<ProductData>
                {
                    new ProductData { Slug = "electronics", Name = "Electronics", Price = 100 },
                    new ProductData { Slug = "hardware", Name = "Hardware", Price = 50 }
                },
                TotalPages = 1
            };

            var repoMock = new Mock<IMakerClient>();
            repoMock.Setup(r => r.ProductGroupsAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Assert
            var sut = new ProductService(repoMock.Object, logger);

            // Act
            var result = await sut.ProductGroupsAsync();

            result.Products.Should().NotBeNull();
            result.Products.Should().HaveCount(2);
            result.Products.First().Name.Should().Be("Electronics");
        }


        [Fact]
        public async Task HandleAsync_WhenRepositoryReturnsNull_ReturnsEmptyResponse()
        {
            // Arrange
            var repoMock = new Mock<IMakerClient>();
            repoMock.Setup(r => r.ProductGroupsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((ProductListResponse?)null);

            // Assert
            var sut = new ProductService(repoMock.Object, logger);

            // Act
            var result = await sut.ProductGroupsAsync();

            result.Should().BeNull();
        }

        [Fact]
        public async Task HandleAsync_WhenCalled_ReturnsProductsByGroup()
        {
            // Arrange
            var expectedResponse = new ProductListResponse
            {
                Products = new List<ProductData>
                {
                    new ProductData { Slug = "electronics", Name = "Electronics", Price = 100 },
                    new ProductData { Slug = "hardware", Name = "Hardware", Price = 50 }
                },
                TotalPages = 1
            };
            var request = new ProductRequest
            {
                Slug = "electronics",
                Page = 1,
                PageSize = 10
            };

            var repoMock = new Mock<IMakerClient>(MockBehavior.Strict);
            repoMock.Setup(r => r.ProductsBySlugAsync(
                    request, It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);


            // Assert
            var sut = new ProductService(repoMock.Object, logger);

            // Act
            var result = await sut.ProductsBySlugAsync(request);

            result.Products.Should().NotBeNull();
            result.Products.Should().HaveCount(2);
            result.Products.First().Name.Should().Be("Electronics");
        }

        [Fact]
        public async Task HandleAsync_WhenRepositoryReturnsNull_ReturnsProductsByGroup()
        {
            // Arrange
            var repoMock = new Mock<IMakerClient>(MockBehavior.Strict);
            repoMock.Setup(r => r.ProductsBySlugAsync(It.IsAny<ProductRequest>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductListResponse?)null);

            var request = new ProductRequest
            {
                Slug = "unknown",
                Page = 1,
                PageSize = 10
            };


            // Assert
            var sut = new ProductService(repoMock.Object, logger);

            // Act
            var result = await sut.ProductsBySlugAsync(request);

            result.Should().BeNull();
        }

        [Fact]
        public async Task HandleAsync_WhenAddCartSucceeds_ReturnsOk()
        {
            AddCartRequest? capturedRequest = null;
            var req = new AddCartRequest
            {
                CartItem = new List<AddToCartItem>
                {
                    new() { Slug = "electronics", Quantity = 2 }
                }
            };

            // Arrange
            var mockRepo = new Mock<IMakerClient>(MockBehavior.Strict);
            mockRepo.Setup(r => r.AddProductsToCartAsync(
                    It.IsAny<AddCartRequest>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                  .Callback<AddCartRequest, string, CancellationToken>((req, key, ct) =>
                  {
                      capturedRequest = req;
                  })
    .Returns(Task.CompletedTask);

            // Assert
            var sut = new ProductService(mockRepo.Object, logger);

            // Act
            await sut.AddProductsToCartAsync(req);
            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.CartItem.Should().HaveCount(1);
            capturedRequest.CartItem.First().Slug.Should().Be("electronics");
            capturedRequest.CartItem.First().Quantity.Should().Be(2);
        }
        [Fact]
        public async Task HandleAsync_WhenClearCartSucceeds_ReturnsNoContent()
        {
            // Arrange
            var mockRepo = new Mock<IMakerClient>(MockBehavior.Strict);
            mockRepo.Setup(r => r.ClearCartAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
            // Assert
            var sut = new ProductService(mockRepo.Object, logger);

            // Act
            var result = await sut.ClearUserCart();
            // Assert
            result.Should().BeTrue(); // method should return true
            mockRepo.Verify(r => r.ClearCartAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }
        [Fact]
        public async Task HandleAsync_WhenCartItemsExist_ReturnsOkWithCartItems()
        {
            // Arrange
            var mockRepo = new Mock<IMakerClient>(MockBehavior.Strict);
            var asset = new DigitalAsset
            {
                Url = "https://example.com/model.glb",
                Is3DFile = true
            };

            var expectedItems = new List<CartItem>
            {
                new CartItem
                {
                    Slug = "electronics",
                    Quantity = 2,
                    Price = 100,
                    Assets = new List<DigitalAsset>(){ asset },
                    BarId = 9684382,
                    Description = "High-end electronics",
                    Name = "Electronics"
                },
                new CartItem
                {
                    Slug = "hardware",
                    Quantity = 1,
                    Price = 50,
                    Assets = new List<DigitalAsset>(){ asset },
                    BarId = 128969,
                    Description = "Durable hardware",
                    Name = "Hardware"
                }
            };
            var expectedReply = new GetCartReply
            {
                CartItem = expectedItems
            };
            mockRepo.Setup(r => r.GetCartAsync(
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedReply);

            var sut = new ProductService(mockRepo.Object, logger);

            // Act
            var result = await sut.GetCartAsync();

            // Assert
            result.Should().NotBeNull();
            result.CartItem.Should().HaveCount(2);
            result.CartItem.First().Slug.Should().Be("electronics");
            result.CartItem.First().Assets.Should().ContainSingle(a => a.Is3DFile && a.Url == "https://example.com/model.glb");
        }

        [Fact]
        public async Task ProductDetailsAsync_WhenProductExists_ReturnsExpectedDetails()
        {
            // Arrange
            var mockRepo = new Mock<IMakerClient>(MockBehavior.Strict);
            var logger = Mock.Of<ILogger<ProductService>>();

            var asset = new DigitalAsset
            {
                Url = "https://example.com/model.glb",
                Is3DFile = true
            };

            var expectedReply = new ProductDetailsReply
            {
                Slug = "electronics",
                Name = "Electronics",
                FriendlyID = "ELEC-001",
                BarID = 123456,
                Description = "High-end electronic product",
                Price = 199.99,
                ReadMe = "Assembly instructions",
                ReadMeHtml = "<p>Assembly instructions</p>",
                Assets = new List<DigitalAsset> { asset }
            };

            var request = new ProductRequest
            {
                Slug = "electronics",
                Page = 1,
                PageSize = 10
            };

            mockRepo.Setup(r => r.ProductDetailsAsync(
                    It.IsAny<ProductRequest>(),
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedReply);

            var sut = new ProductService(mockRepo.Object, logger);

            // Act
            var result = await sut.ProductDetailsAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Slug.Should().Be("electronics");
            result.Name.Should().Be("Electronics");
            result.Price.Should().Be(199.99);
            result.Assets.Should().ContainSingle(a => a.Is3DFile && a.Url == "https://example.com/model.glb");

        }

        [Fact]
        public async Task RemoveProductFromTheCartAsync_WhenCalled_InvokesRepositoryOnce()
        {
            // Arrange
            var mockRepo = new Mock<IMakerClient>(MockBehavior.Strict);
            var logger = Mock.Of<ILogger<ProductService>>();

            var request = new RemoveProductRequest
            {
                Slug = "electronics"
            };

            RemoveProductRequest? capturedRequest = null;

            mockRepo.Setup(r => r.RemoveProductFromTheCartAsync(
                    It.IsAny<RemoveProductRequest>(),
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()))
                .Callback<RemoveProductRequest, string?, CancellationToken>((req, key, ct) =>
                {
                    capturedRequest = req;
                })
                .Returns(Task.CompletedTask);

            var sut = new ProductService(mockRepo.Object, logger);

            // Act
            await sut.RemoveProductFromTheCartAsync(request);

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.Slug.Should().Be("electronics");
        }

    }
}
