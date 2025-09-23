using Maker.RampEdge.Models;

namespace Maker.RampEdge.Services.Contracts;

public interface IProductService
{
    Task<ProductListResponse> GetProductsAsync(ProductListOptions options, CancellationToken cancellationToken = default);
    Task<Product?> GetProductAsync(string sku, CancellationToken cancellationToken = default);
    Task<ProductChangesResponse> GetProductChangesAsync(string? sinceETag = null, DateTime? sinceTimestamp = null, CancellationToken cancellationToken = default);
    Task InvalidateCacheAsync();
    Task<bool> StartDeltaRefreshAsync(TimeSpan interval);
    Task StopDeltaRefreshAsync();

    event Action<ProductChange>? OnProductChanged;
    event Action<List<ProductChange>>? OnProductsChanged;
    event Action<string>? OnError;
}