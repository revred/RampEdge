using FluentAssertions;
using Maker.RampEdge.Configuration;
using Maker.RampEdge.Extensions;
using Maker.RampEdge.Http;
using Maker.RampEdge.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Moq;

namespace Maker.RampEdge.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddRampEdgeClient_WithConfiguration_Should_Register_All_Services()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        var jsRuntimeMock = new Mock<IJSRuntime>();
        services.AddSingleton(jsRuntimeMock.Object);

        services.AddRampEdgeClient(configuration);

        var serviceProvider = services.BuildServiceProvider();

        serviceProvider.GetService<IOptions<RampEdgeSettings>>().Should().NotBeNull();
        serviceProvider.GetService<ITokenStorage>().Should().NotBeNull();
        serviceProvider.GetService<StaticAppHeadersHandler>().Should().NotBeNull();
        serviceProvider.GetService<BearerTokenHandler>().Should().NotBeNull();
        serviceProvider.GetService<IHttpClientFactory>().Should().NotBeNull();
    }

    [Fact]
    public void AddRampEdgeClient_WithAction_Should_Register_All_Services()
    {
        var services = new ServiceCollection();

        var jsRuntimeMock = new Mock<IJSRuntime>();
        services.AddSingleton(jsRuntimeMock.Object);

        services.AddRampEdgeClient(settings =>
        {
            settings.BaseAddress = "https://api.rampedge.com";
            settings.BusinessUnitKey = "test-key";
        });

        var serviceProvider = services.BuildServiceProvider();

        serviceProvider.GetService<IOptions<RampEdgeSettings>>().Should().NotBeNull();
        serviceProvider.GetService<ITokenStorage>().Should().NotBeNull();
        serviceProvider.GetService<StaticAppHeadersHandler>().Should().NotBeNull();
        serviceProvider.GetService<BearerTokenHandler>().Should().NotBeNull();
        serviceProvider.GetService<IHttpClientFactory>().Should().NotBeNull();

        var settings = serviceProvider.GetService<IOptions<RampEdgeSettings>>()!.Value;
        settings.BaseAddress.Should().Be("https://api.rampedge.com");
        settings.BusinessUnitKey.Should().Be("test-key");
    }

    [Fact]
    public void AddRampEdgeClient_Should_Configure_HttpClients()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        var jsRuntimeMock = new Mock<IJSRuntime>();
        services.AddSingleton(jsRuntimeMock.Object);

        services.AddRampEdgeClient(configuration);

        var serviceProvider = services.BuildServiceProvider();
        var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>()!;

        var authClient = httpClientFactory.CreateClient("RampEdgeAuth");
        var apiClient = httpClientFactory.CreateClient("RampEdgeApi");

        authClient.Should().NotBeNull();
        authClient.BaseAddress.Should().Be(new Uri("https://api.rampedge.com/"));

        apiClient.Should().NotBeNull();
        apiClient.BaseAddress.Should().Be(new Uri("https://api.rampedge.com/"));
    }

    [Fact]
    public void AddRampEdgeClient_Should_Throw_When_BaseAddress_Missing()
    {
        var services = new ServiceCollection();
        var configuration = CreateEmptyConfiguration();

        Action act = () => services.AddRampEdgeClient(configuration);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("RampEdge settings missing*");
    }

    [Fact]
    public void AddRampEdgeClient_WithAction_Should_Throw_When_BaseAddress_Empty()
    {
        var services = new ServiceCollection();

        Action act = () => services.AddRampEdgeClient(settings =>
        {
            settings.BaseAddress = "";
            settings.BusinessUnitKey = "test-key";
        });

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("RampEdge BaseAddress is missing.");
    }

    [Fact]
    public void AddRampEdgeClient_Should_Register_TokenStorage_As_Singleton()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        var jsRuntimeMock = new Mock<IJSRuntime>();
        services.AddSingleton(jsRuntimeMock.Object);

        services.AddRampEdgeClient(configuration);

        var serviceProvider = services.BuildServiceProvider();

        var tokenStorage1 = serviceProvider.GetService<ITokenStorage>();
        var tokenStorage2 = serviceProvider.GetService<ITokenStorage>();

        tokenStorage1.Should().BeSameAs(tokenStorage2);
    }

    [Fact]
    public void AddRampEdgeClient_Should_Register_Handlers_As_Transient()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        var jsRuntimeMock = new Mock<IJSRuntime>();
        services.AddSingleton(jsRuntimeMock.Object);

        services.AddRampEdgeClient(configuration);

        var serviceProvider = services.BuildServiceProvider();

        var handler1 = serviceProvider.GetService<StaticAppHeadersHandler>();
        var handler2 = serviceProvider.GetService<StaticAppHeadersHandler>();

        handler1.Should().NotBeSameAs(handler2);
    }

    [Fact]
    public void AddRampEdgeClient_Should_Accept_Client_Configurations()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        var jsRuntimeMock = new Mock<IJSRuntime>();
        services.AddSingleton(jsRuntimeMock.Object);

        var publicClientConfigured = false;
        var secureClientConfigured = false;

        services.AddRampEdgeClient(
            configuration,
            configurePublicClient: builder =>
            {
                publicClientConfigured = true;
            },
            configureSecureClient: builder =>
            {
                secureClientConfigured = true;
            });

        var serviceProvider = services.BuildServiceProvider();

        publicClientConfigured.Should().BeTrue();
        secureClientConfigured.Should().BeTrue();
    }

    private static IConfiguration CreateConfiguration()
    {
        var configData = new Dictionary<string, string?>
        {
            { "RampEdge:BaseAddress", "https://api.rampedge.com" },
            { "RampEdge:BusinessUnitKey", "test-business-unit-key" },
            { "RampEdge:TokenRefreshThresholdMinutes", "5" },
            { "RampEdge:EnableAutoTokenRefresh", "true" }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();
    }

    private static IConfiguration CreateEmptyConfiguration()
    {
        return new ConfigurationBuilder().Build();
    }
}