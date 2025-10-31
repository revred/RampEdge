using Maker.RampEdge;
using Maker.RampEdge.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Maker.RampEdge.Services;

public class BusinessUnitService
{
    private readonly IMakerClient _makerClient;
    private readonly ILogger<BusinessUnitService> _logger;

    public BusinessUnitService(
        [FromKeyedServices("Auth")] IMakerClient authClient,
        ILogger<BusinessUnitService> logger)
    {
        _makerClient = authClient;
        _logger = logger;
    }
    public async Task<StringReply> AboutInfoAsync()
    {
        try
        {
            return await _makerClient.AboutInfoAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching About info.");
            return default!;
        }
    } 
    public async Task<StringReply> GetBlogsAsync()
    {
        try
        {
            return await _makerClient.GetBlogsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching About info.");
            return default!;
        }
    }
    public async Task<StringReply> GetDocumentationAsync()
    {
        try
        {
            return await _makerClient.GetDocumentationAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching About info.");
            return default!;
        }
    }
    public async Task<StringReply> GetServicesAsync()
    {
        try
        {
            return await _makerClient.GetServicesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching About info.");
            return default!;
        }
    }


}
