using Maker.RampEdge;
using Maker.RampEdge.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Maker.RampEdge.Services;

public class ReportService
{
    private readonly IMakerClient _makerClient;
    private readonly ILogger<ReportService> _logger;

    public ReportService(
        [FromKeyedServices("Auth")] IMakerClient authClient,
        ILogger<ReportService> logger)
    {
        _makerClient = authClient;
        _logger = logger;
    }
    public async Task AddProductReportAsync(AddProductReportRequest body)
    {
        try
        {
            await _makerClient.AddProductReportAsync(body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding product report for {ProductBarID}", body.ProductBarID);
        }
    }

    public async Task<ReportListResponse> GetReportByProductAsync(ReportAndRatingRequest body)
    {
        try
        {
            return await _makerClient.GetReportByProductAsync(body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching reports for {BarID}", body.BarID);
            return default!;
        }
    }

    public async Task UpdateProductReportAsync(UpdateProductReportRequest body)
    {
        try
        {
            await _makerClient.UpdateProductReportAsync(body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product report {ReportBarID}", body.ReportBarID);
        }
    }

}
