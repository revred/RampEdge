using FluentAssertions;
using Maker.RampEdge.Services;
using Microsoft.JSInterop;
using Moq;

namespace Maker.RampEdge.Tests.Services;

public class TokenStorageTests
{
    private readonly Mock<IJSRuntime> _jsRuntimeMock;
    private readonly TokenStorage _tokenStorage;

    public TokenStorageTests()
    {
        _jsRuntimeMock = new Mock<IJSRuntime>();
        _tokenStorage = new TokenStorage(_jsRuntimeMock.Object);
    }

    [Fact]
    public void TokenStorage_Should_Accept_JSRuntime_Dependency()
    {
        var jsRuntime = new Mock<IJSRuntime>().Object;

        var tokenStorage = new TokenStorage(jsRuntime);

        tokenStorage.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAsync_Should_Return_Value_When_Mocked()
    {
        var key = "test-key";
        var expectedValue = "test-value";

        _jsRuntimeMock
            .Setup(js => js.InvokeAsync<string>(
                "localStorage.getItem",
                key))
            .ReturnsAsync(expectedValue);

        var result = await _tokenStorage.GetAsync(key);

        result.Should().Be(expectedValue);
    }

    [Fact]
    public async Task GetAsync_Should_Return_Null_When_Mocked_As_Null()
    {
        var key = "non-existent-key";

        _jsRuntimeMock
            .Setup(js => js.InvokeAsync<string>(
                "localStorage.getItem",
                key))
            .ReturnsAsync((string?)null);

        var result = await _tokenStorage.GetAsync(key);

        result.Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_Should_Not_Throw()
    {
        var key = "test-key";
        var value = "test-value";

        var act = async () => await _tokenStorage.SetAsync(key, value);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task RemoveAsync_Should_Not_Throw()
    {
        var key = "test-key";

        var act = async () => await _tokenStorage.RemoveAsync(key);

        await act.Should().NotThrowAsync();
    }
}