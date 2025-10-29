using Maker.RampEdge;
using Maker.RampEdge.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Maker.RampEdge.Services;

public class StripeService
{
    private readonly IMakerClient _makerClient;
    private readonly ILogger<StripeService> _logger;

    public StripeService(
        [FromKeyedServices("Auth")] IMakerClient authClient,
        ILogger<StripeService> logger)
    {
        _makerClient = authClient;
        _logger = logger;
    }

    public async Task<CreateSessionResponse> CreateCheckoutSessionAsync(CreateSessionRequest body)
    {
        try
        {
            return await _makerClient.CreateCheckoutSessionAsync(body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating checkout session for {Email}", body.EmailAddress);
            return default!;
        }
    }

    public async Task StripeWebhookAsync()
    {
        try
        {
            await _makerClient.StripeWebhookAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling Stripe webhook");
        }
    }
}
