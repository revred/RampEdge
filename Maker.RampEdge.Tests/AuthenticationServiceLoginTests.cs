using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Maker.RampEdge;
using Maker.RampEdge.Services;
using Maker.RampEdge.Services.Contracts;
using Maker.RampEdge.Tests.Helper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Maker.RampEdge.Tests;

public class AuthenticationServiceLoginTests
{
    [Fact]
    public async Task LoginAsync_WhenServerReturnsToken_StoresTokensAndUpdatesState()
    {
        // Arrange
        var email = "demo@maker.ai";
        var password = "test@123456789";
        var jwt = JwtTestTokens.ValidWithEmail(3600, email);
        var refresh = "REFRESH_123";

        var makerClient = new Mock<IMakerClient>(MockBehavior.Strict);
        makerClient
            .Setup(c => c.LoginAsync(
                It.Is<LoginRequest>(r => r.EmailAndPassword == $"{email}|{password}"),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LoginResponse
            {
                Token = jwt,
                RefreshToken = refresh,
                IsMakerAI = true
            });

        var storage = new InMemoryTokenStorage(); // your test double
        var logger = NullLogger<AuthenticationService>.Instance;

        var sut = new AuthenticationService(makerClient.Object, storage, logger);

        // Act
        var ok = await sut.LoginAsync(email, password);

        // Assert
        ok.Should().BeTrue();
        sut.IsAuthenticated.Should().BeTrue();
        sut.UserName.Should().Be(email);

        // Token is retrievable and has an expiry
        (await sut.GetAccessTokenAsync()).Should().Be(jwt);
        (await sut.GetAccessTokenExpiryAsync()).Should().NotBeNull();

        makerClient.VerifyAll();
    }

    [Fact]
    public async Task LoginAsync_WhenServerReturnsEmptyToken_ReturnsFalse_AndKeepsStateUnauthenticated()
    {
        // Arrange
        var email = "demo@maker.ai";
        var password = "wrong";

        var makerClient = new Mock<IMakerClient>(MockBehavior.Strict);
        makerClient
            .Setup(c => c.LoginAsync(
                It.Is<LoginRequest>(r => r.EmailAndPassword == $"{email}|{password}"),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LoginResponse
            {
                Token = "", 
                RefreshToken = null,
                IsMakerAI = false
            });

        var storage = new InMemoryTokenStorage();
        var logger = NullLogger<AuthenticationService>.Instance;

        var sut = new AuthenticationService(makerClient.Object, storage, logger);

        // Act
        var ok = await sut.LoginAsync(email, password);

        // Assert
        ok.Should().BeFalse();
        sut.IsAuthenticated.Should().BeFalse();
        sut.UserName.Should().BeNull();

        makerClient.VerifyAll();
    }

    [Fact]
    public async Task UserAsync_WhenCalled_InvokesClientOnce()
    {
        // Arrange
        var logger = Mock.Of<ILogger<AuthenticationService>>();
        var storage = new InMemoryTokenStorage();
        var mockClient = new Mock<IMakerClient>(MockBehavior.Strict);

        var request = new UserRequest
        {
            UserDetail = "John Doe, johndoe@example.com"
        };

        mockClient.Setup(c => c.UserAsync(
                It.IsAny<UserRequest>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var sut = new AuthenticationService(mockClient.Object,storage, logger);

        // Act
        await sut.UserAsync(request);

        // Assert
        mockClient.Verify(c => c.UserAsync(
            It.Is<UserRequest>(r => r.UserDetail == "John Doe, johndoe@example.com"),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
