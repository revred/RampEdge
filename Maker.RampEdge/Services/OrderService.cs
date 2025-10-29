using Maker.RampEdge;
using Maker.RampEdge.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Maker.RampEdge.Services;

public class OrderService
{
    private readonly IMakerClient _makerClient;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        [FromKeyedServices("Auth")] IMakerClient authClient,
        ILogger<OrderService> logger)
    {
        _makerClient = authClient;
        _logger = logger;
    }

    public async Task<OrderResponse> GetAllOrdersAsync(OrderRequest request)
    {
        try
        {
            return await _makerClient.GetAllOrdersAsync(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return default!;
        }
    }
    public async Task CancelProductOrderAsync(CancelProductOrderRequest request)
    {
        try
        {
            await _makerClient.MarkerRestServiceEndpointsOrderCancelProductOrderAsync(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }

}
