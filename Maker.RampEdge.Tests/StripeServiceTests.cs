using FluentAssertions;
using Maker.RampEdge.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Maker.RampEdge.Tests
{
    public class StripeServiceTests
    {
        ILogger<StripeService> logger = NullLogger<StripeService>.Instance;
        [Fact]
        public async Task CreateCheckoutSessionAsync_WhenCalled_ReturnsExpectedResponse()
        {
            // Arrange
            var mockClient = new Mock<IMakerClient>(MockBehavior.Strict);

            var request = new CreateSessionRequest
            {
                EmailAddress = "test@example.com",
                Address = 0,
                BusinessUnitKey = "unit-001",
                Items = new List<Item>
            {
                new() { BarId = 111, Slug = "item1", Quantity = 2, Price = 100 },
                new() { BarId = 222, Slug = "item2", Quantity = 1, Price = 200 }
            }
            };

            var expectedResponse = new CreateSessionResponse
            {
                ClientSecret = "sk_test_secret_123"
            };

            mockClient.Setup(c => c.CreateCheckoutSessionAsync(
                    It.IsAny<CreateSessionRequest>(),
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var sut = new StripeService(mockClient.Object, logger);

            // Act
            var result = await sut.CreateCheckoutSessionAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.ClientSecret.Should().Be("sk_test_secret_123");
        }
        [Fact]
        public async Task StripeWebhookAsync_WhenCalled_InvokesClientOnce()
        {
            // Arrange
            var mockClient = new Mock<IMakerClient>(MockBehavior.Strict);

            mockClient.Setup(c => c.StripeWebhookAsync(
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var sut = new StripeService(mockClient.Object, logger);

            // Act
            await sut.StripeWebhookAsync();

            // Assert
            mockClient.Verify(c => c.StripeWebhookAsync(
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

    }
}
