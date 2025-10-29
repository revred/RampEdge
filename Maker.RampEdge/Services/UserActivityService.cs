using Maker.RampEdge;
using Maker.RampEdge.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Maker.RampEdge.Services;

public class UserActivityService
{
    private readonly IMakerClient _makerClient;
    private readonly ILogger<UserActivityService> _logger;

    public UserActivityService(
        [FromKeyedServices("Auth")] IMakerClient authClient,
        ILogger<UserActivityService> logger)
    {
        _makerClient = authClient;
        _logger = logger;
    }
    public async Task Insert_User_ActivityAsync(UserActivityRequest body)
    {
        try
        {
            await _makerClient.Insert_User_ActivityAsync(body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inserting user activity for {UserID}", body.UserID);
        }
    }
}
