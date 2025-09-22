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
    public async Task GetAsync_WithExistingKey_ShouldReturnValue()
    {
        // Arrange
        var key = "existing-key";
        var expectedValue = "stored-value";

        _jsRuntimeMock
            .Setup(js => js.InvokeAsync<string>("localStorage.getItem", It.Is<object[]>(args => args.Length == 1 && args[0].Equals(key))))
            .Returns(ValueTask.FromResult(expectedValue));

        // Act
        var result = await _tokenStorage.GetAsync(key);

        // Assert
        result.Should().Be(expectedValue);
        _jsRuntimeMock.Verify(
            js => js.InvokeAsync<string>("localStorage.getItem", key),
            Times.Once);
    }

    [Fact]
    public async Task GetAsync_WithNonExistentKey_ShouldReturnNull()
    {
        // Arrange
        var key = "non-existent-key";

        _jsRuntimeMock
            .Setup(js => js.InvokeAsync<string>("localStorage.getItem", key))
            .Returns(ValueTask.FromResult((string?)null)!);

        // Act
        var result = await _tokenStorage.GetAsync(key);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_WithValidKeyAndValue_ShouldStoreValue()
    {
        // Arrange
        var key = "test-key";
        var value = "test-value";

        _jsRuntimeMock
            .Setup(js => js.InvokeVoidAsync("localStorage.setItem", key, value))
            .Returns(ValueTask.CompletedTask);

        // Act
        await _tokenStorage.SetAsync(key, value);

        // Assert
        _jsRuntimeMock.Verify(
            js => js.InvokeVoidAsync("localStorage.setItem", key, value),
            Times.Once);
    }

    [Fact]
    public async Task SetAsync_WithNullValue_ShouldStoreNull()
    {
        // Arrange
        var key = "test-key";
        string? value = null;

        _jsRuntimeMock
            .Setup(js => js.InvokeVoidAsync("localStorage.setItem", It.IsAny<string>(), It.IsAny<string>()))
            .Returns(ValueTask.CompletedTask);

        // Act
        await _tokenStorage.SetAsync(key, value);

        // Assert
        _jsRuntimeMock.Verify(
            js => js.InvokeVoidAsync("localStorage.setItem", key, value),
            Times.Once);
    }

    [Fact]
    public async Task SetAsync_WithEmptyValue_ShouldStoreEmptyString()
    {
        // Arrange
        var key = "test-key";
        var value = string.Empty;

        _jsRuntimeMock
            .Setup(js => js.InvokeVoidAsync("localStorage.setItem", It.IsAny<string>(), It.IsAny<string>()))
            .Returns(ValueTask.CompletedTask);

        // Act
        await _tokenStorage.SetAsync(key, value);

        // Assert
        _jsRuntimeMock.Verify(
            js => js.InvokeVoidAsync("localStorage.setItem", key, value),
            Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_WithValidKey_ShouldRemoveValue()
    {
        // Arrange
        var key = "key-to-remove";

        _jsRuntimeMock
            .Setup(js => js.InvokeVoidAsync("localStorage.removeItem", key))
            .Returns(ValueTask.CompletedTask);

        // Act
        await _tokenStorage.RemoveAsync(key);

        // Assert
        _jsRuntimeMock.Verify(
            js => js.InvokeVoidAsync("localStorage.removeItem", key),
            Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task SetAsync_WithInvalidKey_ShouldThrowArgumentException(string? key)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _tokenStorage.SetAsync(key!, "value"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task GetAsync_WithInvalidKey_ShouldThrowArgumentException(string? key)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _tokenStorage.GetAsync(key!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task RemoveAsync_WithInvalidKey_ShouldThrowArgumentException(string? key)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _tokenStorage.RemoveAsync(key!));
    }

    [Fact]
    public async Task SetAsync_WhenJSRuntimeThrows_ShouldPropagateException()
    {
        // Arrange
        var key = "test-key";
        var value = "test-value";

        _jsRuntimeMock
            .Setup(js => js.InvokeVoidAsync("localStorage.setItem", It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new JSException("JavaScript error"));

        // Act & Assert
        await Assert.ThrowsAsync<JSException>(async () => await _tokenStorage.SetAsync(key, value));
    }

    [Fact]
    public async Task GetAsync_WhenJSRuntimeThrows_ShouldPropagateException()
    {
        // Arrange
        var key = "test-key";

        _jsRuntimeMock
            .Setup(js => js.InvokeAsync<string>("localStorage.getItem", It.IsAny<string>()))
            .ThrowsAsync(new JSException("JavaScript error"));

        // Act & Assert
        await Assert.ThrowsAsync<JSException>(async () => await _tokenStorage.GetAsync(key));
    }

    [Fact]
    public async Task RemoveAsync_WhenJSRuntimeThrows_ShouldPropagateException()
    {
        // Arrange
        var key = "test-key";

        _jsRuntimeMock
            .Setup(js => js.InvokeVoidAsync("localStorage.removeItem", It.IsAny<string>()))
            .ThrowsAsync(new JSException("JavaScript error"));

        // Act & Assert
        await Assert.ThrowsAsync<JSException>(async () => await _tokenStorage.RemoveAsync(key));
    }

    [Fact]
    public async Task TokenStorage_RoundTrip_ShouldWorkCorrectly()
    {
        // Arrange
        var key = "roundtrip-key";
        var value = "roundtrip-value";

        _jsRuntimeMock
            .Setup(js => js.InvokeVoidAsync("localStorage.setItem", It.IsAny<string>(), It.IsAny<string>()))
            .Returns(ValueTask.CompletedTask);

        _jsRuntimeMock
            .Setup(js => js.InvokeAsync<string>("localStorage.getItem", key))
            .Returns(ValueTask.FromResult(value));

        // Act
        await _tokenStorage.SetAsync(key, value);
        var result = await _tokenStorage.GetAsync(key);

        // Assert
        result.Should().Be(value);
    }
}