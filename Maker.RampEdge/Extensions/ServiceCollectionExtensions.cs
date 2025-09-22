using Maker.RampEdge.Configuration;
using Maker.RampEdge.Http;
using Maker.RampEdge.Services;
using Maker.RampEdge.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Maker.RampEdge.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRampEdgeClient(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IHttpClientBuilder>? configurePublicClient = null,
        Action<IHttpClientBuilder>? configureSecureClient = null,
        Func<HttpRequestMessage, Task>? onUnauthorized = null)
    {
        services.Configure<RampEdgeSettings>(o => configuration.GetSection(RampEdgeSettings.SectionName).Bind(o));
        var rampEdgeSettings = configuration.GetSection(RampEdgeSettings.SectionName).Get<RampEdgeSettings>()
            ?? throw new InvalidOperationException($"RampEdge settings missing ('{RampEdgeSettings.SectionName}').");
        if (string.IsNullOrWhiteSpace(rampEdgeSettings.BaseAddress))
            throw new InvalidOperationException("RampEdge BaseAddress is missing.");

        services.AddSingleton<ITokenStorage, TokenStorage>();
        services.AddSingleton<IAuthenticationService, AuthenticationService>();

        services.AddTransient<StaticAppHeadersHandler>();
        services.AddTransient(sp => new BearerTokenHandler(
            sp.GetRequiredService<IAuthenticationService>(),
            onUnauthorized));

        var authClientBuilder = services.AddHttpClient("RampEdgeAuth", (sp, c) =>
        {
            c.BaseAddress = new Uri(rampEdgeSettings.BaseAddress);
            c.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddHttpMessageHandler<StaticAppHeadersHandler>();

        configurePublicClient?.Invoke(authClientBuilder);

        var apiClientBuilder = services.AddHttpClient("RampEdgeApi", (sp, c) =>
        {
            c.BaseAddress = new Uri(rampEdgeSettings.BaseAddress);
            c.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddHttpMessageHandler<StaticAppHeadersHandler>()
        .AddHttpMessageHandler<BearerTokenHandler>();

        configureSecureClient?.Invoke(apiClientBuilder);

#if DEBUG
        services.AddLogging();
#endif

        return services;
    }

    public static IServiceCollection AddRampEdgeClient(
        this IServiceCollection services,
        Action<RampEdgeSettings> configureSettings,
        Action<IHttpClientBuilder>? configurePublicClient = null,
        Action<IHttpClientBuilder>? configureSecureClient = null,
        Func<HttpRequestMessage, Task>? onUnauthorized = null)
    {
        var settings = new RampEdgeSettings();
        configureSettings(settings);

        if (string.IsNullOrWhiteSpace(settings.BaseAddress))
            throw new InvalidOperationException("RampEdge BaseAddress is missing.");

        services.Configure<RampEdgeSettings>(o => configureSettings(o));

        services.AddSingleton<ITokenStorage, TokenStorage>();
        services.AddSingleton<IAuthenticationService, AuthenticationService>();

        services.AddTransient<StaticAppHeadersHandler>();
        services.AddTransient(sp => new BearerTokenHandler(
            sp.GetRequiredService<IAuthenticationService>(),
            onUnauthorized));

        var authClientBuilder = services.AddHttpClient("RampEdgeAuth", (sp, c) =>
        {
            c.BaseAddress = new Uri(settings.BaseAddress);
            c.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddHttpMessageHandler<StaticAppHeadersHandler>();

        configurePublicClient?.Invoke(authClientBuilder);

        var apiClientBuilder = services.AddHttpClient("RampEdgeApi", (sp, c) =>
        {
            c.BaseAddress = new Uri(settings.BaseAddress);
            c.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddHttpMessageHandler<StaticAppHeadersHandler>()
        .AddHttpMessageHandler<BearerTokenHandler>();

        configureSecureClient?.Invoke(apiClientBuilder);

#if DEBUG
        services.AddLogging();
#endif

        return services;
    }
}