using Maker.RampEdge;
using Maker.RampEdge.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Maker.RampEdge.Services;

public class AddressService
{
    private readonly IMakerClient _makerClient;
    private readonly ILogger<AddressService> _logger;

    public AddressService(
        [FromKeyedServices("Auth")] IMakerClient authClient,
        ILogger<AddressService> logger)
    {
        _makerClient = authClient;
        _logger = logger;
    }

    public async Task<AddressResponse> GetAddressAsync()
    {
        try
        {
            return await _makerClient.GetAddressAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching addresses");
            return default!;
        }
    }
    

    public async Task<UpsertAddressResponse> UpsertAddressAsync(AddressRequest body)
    {
        try
        {
            return await _makerClient.UpsertAddressAsync(body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return default!;
        }
    }

}
