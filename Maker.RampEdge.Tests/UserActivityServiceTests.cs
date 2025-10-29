using FluentAssertions;
using Maker.RampEdge.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Maker.RampEdge.Tests
{
    public class UserActivityServiceTests
    {
        ILogger<UserActivityService> logger = NullLogger<UserActivityService>.Instance;
        [Fact]
        public async Task Insert_User_ActivityAsync_WhenCalled_InvokesClientWithCorrectRequest()
        {
            // Arrange
            var mockClient = new Mock<IMakerClient>(MockBehavior.Strict);
            UserActivityRequest? capturedRequest = null;

            var request = new UserActivityRequest
            {
                UserID = "user123",
                BusinessUnitId = 456,
                EventType = "Login",
                MetaData = "User logged in successfully"
            };

            mockClient.Setup(c => c.Insert_User_ActivityAsync(
                    It.IsAny<UserActivityRequest>(),
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()))
                .Callback<UserActivityRequest, string?, CancellationToken>((r, k, ct) => capturedRequest = r)
                .Returns(Task.CompletedTask);

            var sut = new UserActivityService(mockClient.Object, logger);

            // Act
            await sut.Insert_User_ActivityAsync(request);

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.UserID.Should().Be("user123");
            capturedRequest.EventType.Should().Be("Login");
            capturedRequest.MetaData.Should().Be("User logged in successfully");

        }

    }
}
