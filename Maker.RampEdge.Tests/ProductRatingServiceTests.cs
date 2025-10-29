using FluentAssertions;
using Maker.RampEdge.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Maker.RampEdge.Tests
{
    public class ProductRatingServiceTests
    {
        ILogger<ProductRatingService> logger = NullLogger<ProductRatingService>.Instance;
        [Fact]
        public async Task AddRatingAsync_WhenCalled_InvokesRepositoryWithCorrectData()
        {
            // Arrange
            var mockRepo = new Mock<IMakerClient>(MockBehavior.Strict);

            AddRatingRequest? capturedRequest = null;

            var file = new FileString
            {
                FileBytes = Convert.ToBase64String(new byte[] { 1, 2, 3 }),
                FileName = "image.png"
            };

            var request = new AddRatingRequest
            {
                ProductBarID = 123456,
                Message = "Great product!",
                RateCount = 5,
                Files = new List<FileString> { file }
            };

            mockRepo.Setup(r => r.AddRatingAsync(
                    It.IsAny<AddRatingRequest>(),
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()))
                .Callback<AddRatingRequest, string?, CancellationToken>((req, key, ct) => capturedRequest = req)
                .Returns(Task.CompletedTask);

            var sut = new ProductRatingService(mockRepo.Object, logger);

            // Act
            await sut.AddRatingAsync(request);

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.ProductBarID.Should().Be(123456);
            capturedRequest.Message.Should().Be("Great product!");
            capturedRequest.RateCount.Should().Be(5);
            capturedRequest.Files.Should().ContainSingle(f => f.FileName == "image.png");
        }
        [Fact]
        public async Task GetRatingByProductAsync_WhenCalled_ReturnsExpectedRatings()
        {
            // Arrange
            var mockRepo = new Mock<IMakerClient>(MockBehavior.Strict);

            var asset = new DigitalAsset
            {
                Url = "https://example.com/review.png",
                Is3DFile = false
            };

            var expectedResponse = new RatingResponse
            {
                Ratings = new List<Rating>
        {
            new Rating
            {
                RateCount = 5,
                Message = "Excellent product!",
                Assets = new List<DigitalAsset> { asset }
            },
            new Rating
            {
                RateCount = 4,
                Message = "Good quality",
                Assets = new List<DigitalAsset>()
            }
        }
            };

            var request = new ReportAndRatingRequest { BarID = 123456 };

            mockRepo.Setup(r => r.GetRatingByProductAsync(
                    It.IsAny<ReportAndRatingRequest>(),
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var sut = new ProductRatingService(mockRepo.Object, logger);

            // Act
            var result = await sut.GetRatingByProductAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Ratings.Should().HaveCount(2);
            result.Ratings.First().Message.Should().Be("Excellent product!");
            result.Ratings.First().Assets.Should().ContainSingle(a => a.Url == "https://example.com/review.png");
        }


    }
}
