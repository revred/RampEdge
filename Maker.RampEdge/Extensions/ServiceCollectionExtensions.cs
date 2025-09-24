using Maker.RampEdge.Configuration;
using Maker.RampEdge.Http;
using Maker.RampEdge.Services;
using Maker.RampEdge.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Maker.RampEdge.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMakerClient(
      this IServiceCollection services,
      IConfiguration configuration,
      Action<IHttpClientBuilder>? configurePublicClient = null, // optional customization of the Api client
      Action<IHttpClientBuilder>? configureSecureClient = null, // optional customization of the Api client
      Func<HttpRequestMessage, Task>? onUnauthorized = null
  )
    {
        // Bind settings
        services.Configure<RampEdgeSettings>(o => configuration.GetSection(RampEdgeSettings.SectionName).Bind(o));
        var makerSettings = configuration.GetSection(RampEdgeSettings.SectionName).Get<RampEdgeSettings>()
            ?? throw new InvalidOperationException($"Maker settings missing ('{RampEdgeSettings.SectionName}').");
        if (string.IsNullOrWhiteSpace(makerSettings.BaseAddress))
            throw new InvalidOperationException("Maker BaseAddress is missing.");
        // Core services
        services.AddSingleton<ITokenStorage, TokenStorage>();
        services.AddSingleton<IAuthenticationService, AuthenticationService>();

        // Handlers
        services.AddTransient<StaticAppHeadersHandler>();
        services.AddTransient(sp => new BearerTokenHandler(
            sp.GetRequiredService<IAuthenticationService>(),
            onUnauthorized // inject callback here
        ));

        services.AddHttpClient("Auth", (sp, c) =>
        {
            c.BaseAddress = new Uri(makerSettings.BaseAddress);
            c.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddHttpMessageHandler<StaticAppHeadersHandler>();

        services.AddHttpClient("Api", (sp, c) =>
        {
            c.BaseAddress = new Uri(makerSettings.BaseAddress);
            c.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddHttpMessageHandler<StaticAppHeadersHandler>()
        .AddHttpMessageHandler<BearerTokenHandler>();

        services.AddTransient<IMakerClient>(sp =>
            new MakerClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api")));

        services.AddKeyedTransient<IMakerClient>("Auth", (sp, key) =>
            new MakerClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient("Auth")));


#if DEBUG
        services.AddLogging();
#endif

        return services;
    }
}