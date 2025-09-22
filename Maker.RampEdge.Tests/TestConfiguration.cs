using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Maker.RampEdge.Extensions;
using Maker.RampEdge.Services.Contracts;

namespace Maker.RampEdge.Tests;

public class TestConfiguration
{
    public static IServiceProvider CreateServiceProvider()
    {
        var configuration = CreateConfiguration();
        var services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        services.AddSingleton<IConfiguration>(configuration);

        services.AddRampEdgeClient(configuration);

        return services.BuildServiceProvider();
    }

    public static IConfiguration CreateConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        var config = builder.Build();

        if (config.GetValue<bool>("TestSettings:UseRealApi"))
        {
            builder.AddUserSecrets<TestConfiguration>();
        }

        return builder.Build();
    }

    public static class TestUsers
    {
        public const string ValidEmail = "test@rampedge.com";
        public const string ValidPassword = "TestPassword123!";
        public const string InvalidEmail = "invalid@test.com";
        public const string InvalidPassword = "wrongpassword";
    }

    public static class TestProducts
    {
        public const string ValidProductId = "test-product-1";
        public const string ValidProductSlug = "rampedge-product";
        public const string InvalidProductId = "nonexistent-product";
    }

    public static class TestData
    {
        public const string BusinessUnitKey = "test-business-unit-key";
        public const int DefaultTimeout = 30000;
    }
}