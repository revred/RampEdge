using FluentAssertions;
using Maker.RampEdge.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Maker.RampEdge.Tests
{
    public class BusinessUnitServiceTests
    {
        ILogger<BusinessUnitService> logger = NullLogger<BusinessUnitService>.Instance;
        [Fact]
        public async Task AboutInfoAsync_WhenCalled_ReturnsExpectedMessage()
        {
            // Arrange
            var mockRepo = new Mock<IMakerClient>(MockBehavior.Strict);

            var expectedReply = new StringReply { Detail = "About Maker Platform" };

            mockRepo.Setup(r => r.AboutInfoAsync(
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedReply);

            var sut = new BusinessUnitService(mockRepo.Object, logger);

            // Act
            var result = await sut.AboutInfoAsync();

            // Assert
            result.Should().NotBeNull();
            result.Detail.Should().Be("About Maker Platform");
        }
        [Fact]
        public async Task GetBlogsAsync_WhenCalled_ReturnsExpectedMessage()
        {
            // Arrange
            var mockRepo = new Mock<IMakerClient>(MockBehavior.Strict);

            var expectedReply = new StringReply { Detail = "Latest Blog Posts" };

            mockRepo.Setup(r => r.GetBlogsAsync(
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedReply);

            var sut = new BusinessUnitService(mockRepo.Object, logger);

            // Act
            var result = await sut.GetBlogsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Detail.Should().Be("Latest Blog Posts");

            mockRepo.Verify(r => r.GetBlogsAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetDocumentationAsync_WhenCalled_ReturnsExpectedMessage()
        {
            // Arrange
            var mockRepo = new Mock<IMakerClient>(MockBehavior.Strict);

            var expectedReply = new StringReply { Detail = "API Documentation" };

            mockRepo.Setup(r => r.GetDocumentationAsync(
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedReply);

            var sut = new BusinessUnitService(mockRepo.Object, logger);

            // Act
            var result = await sut.GetDocumentationAsync();

            // Assert
            result.Should().NotBeNull();
            result.Detail.Should().Be("API Documentation");

        }

        [Fact]
        public async Task GetServicesAsync_WhenCalled_ReturnsExpectedMessage()
        {
            // Arrange
            var mockRepo = new Mock<IMakerClient>(MockBehavior.Strict);

            var expectedReply = new StringReply { Detail = "Available Services" };

            mockRepo.Setup(r => r.GetServicesAsync(
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedReply);

            var sut = new BusinessUnitService(mockRepo.Object, logger);

            // Act
            var result = await sut.GetServicesAsync();

            // Assert
            result.Should().NotBeNull();
            result.Detail.Should().Be("Available Services");

        }

    }

}
