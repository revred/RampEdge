using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Maker.RampEdge.Services;

public class ProductRatingService
{
    private readonly IMakerClient _makerClient;
    private readonly ILogger<ProductRatingService> _logger;

    public ProductRatingService(
        [FromKeyedServices("Auth")] IMakerClient authClient,
        ILogger<ProductRatingService> logger)
    {
        _makerClient = authClient;
        _logger = logger;
    }
    public async Task AddRatingAsync(AddRatingRequest body)
    {
        try
        {
            await _makerClient.AddRatingAsync(body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding rating for product {ProductBarID}", body.ProductBarID);
        }
    }

    public async Task<RatingResponse> GetRatingByProductAsync(ReportAndRatingRequest body)
    {
        try
        {
            return await _makerClient.GetRatingByProductAsync(body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching ratings for product {BarID}", body.BarID);
            return default!;
        }
    }



}
