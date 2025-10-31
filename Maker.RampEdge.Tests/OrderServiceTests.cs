using FluentAssertions;
using Maker.RampEdge.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Maker.RampEdge.Tests
{
    public class OrderServiceTests
    {
        ILogger<OrderService> logger = NullLogger<OrderService>.Instance;
        [Fact]
        public async Task GetAllOrdersAsync_WhenCalled_ReturnsExpectedOrders()
        {
            // Arrange
            var mockRepo = new Mock<IMakerClient>(MockBehavior.Strict);

            var expectedResponse = new OrderResponse
            {
                TotalPages = 2,
                OrderDetails = new List<OrderDetail>
        {
            new()
            {
                OrderBarID = "ORD001",
                Status = "Pending",
                CreatedDate = 1729833600000, // Example timestamp
                Price = 500,
                ProductItems = new List<ProductItem>
                {
                    new() { Slug = "electronics", Name = "Smart TV", Quantity = 1, Price = 500 }
                }
            },
            new()
            {
                OrderBarID = "ORD002",
                Status = "Completed",
                CreatedDate = 1729747200000,
                Price = 300,
                ProductItems = new List<ProductItem>
                {
                    new() { Slug = "hardware", Name = "Drill Machine", Quantity = 1, Price = 300 }
                }
            }
        }
            };

            var request = new OrderRequest { Page = 1, PageSize = 10 };

            mockRepo.Setup(r => r.GetAllOrdersAsync(
                    It.IsAny<OrderRequest>(),
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var sut = new OrderService(mockRepo.Object, logger);

            // Act
            var result = await sut.GetAllOrdersAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.OrderDetails.Should().HaveCount(2);
            result.OrderDetails.First().OrderBarID.Should().Be("ORD001");
        }

        [Fact]
        public async Task CancelProductOrderAsync_WhenCalled_InvokesRepositoryOnce()
        {
            // Arrange
            var mockRepo = new Mock<IMakerClient>(MockBehavior.Strict);

            var request = new CancelProductOrderRequest { BarId = 2369848 };

            CancelProductOrderRequest? capturedRequest = null;

            mockRepo.Setup(r => r.MarkerRestServiceEndpointsOrderCancelProductOrderAsync(
                    It.IsAny<CancelProductOrderRequest>(),
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()))
                .Callback<CancelProductOrderRequest, string?, CancellationToken>((req, key, ct) => capturedRequest = req)
                .Returns(Task.CompletedTask);

            var sut = new OrderService(mockRepo.Object, logger);

            // Act
            await sut.CancelProductOrderAsync(request);

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.BarId.Should().Be(2369848);
        }

    }
}
