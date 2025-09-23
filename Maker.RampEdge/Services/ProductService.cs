using Maker.RampEdge.Models;
using Maker.RampEdge.Services.Contracts;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Maker.RampEdge.Services;

public class ProductService : IProductService, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductService> _logger;
    private readonly ConcurrentDictionary<string, ProductCacheEntry> _productCache = new();
    private readonly ConcurrentDictionary<string, ProductListCacheEntry> _listCache = new();
    private readonly SemaphoreSlim _cacheLock = new(1, 1);
    private readonly Timer? _deltaRefreshTimer;
    private string _lastETag = string.Empty;
    private bool _disposed;

    private static readonly TimeSpan DefaultCacheExpiry = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan DefaultDeltaInterval = TimeSpan.FromSeconds(2);

    public event Action<ProductChange>? OnProductChanged;
    public event Action<List<ProductChange>>? OnProductsChanged;
    public event Action<string>? OnError;

    public ProductService(IHttpClientFactory httpClientFactory, ILogger<ProductService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("RampEdgeApi");
        _logger = logger;
    }

    public async Task<ProductListResponse> GetProductsAsync(ProductListOptions options, CancellationToken cancellationToken = default)
    {
        var cacheKey = GenerateListCacheKey(options);

        await _cacheLock.WaitAsync(cancellationToken);
        try
        {
            // Check cache first
            if (_listCache.TryGetValue(cacheKey, out var cached) && cached.ExpiresAt > DateTime.UtcNow)
            {
                _logger.LogDebug("Returning cached product list for key: {CacheKey}", cacheKey);
                return cached.Response;
            }

            // Build query string
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(options.Category))
                queryParams.Add($"category={Uri.EscapeDataString(options.Category)}");
            if (!string.IsNullOrEmpty(options.Search))
                queryParams.Add($"search={Uri.EscapeDataString(options.Search)}");
            queryParams.Add($"page={options.Page}");
            queryParams.Add($"pageSize={options.PageSize}");

            var queryString = string.Join("&", queryParams);
            var url = $"/v1/products?{queryString}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (cached != null)
            {
                request.Headers.Add("If-None-Match", cached.ETag);
            }

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotModified && cached != null)
            {
                // Update cache expiry
                cached.ExpiresAt = DateTime.UtcNow.Add(DefaultCacheExpiry);
                _logger.LogDebug("Product list not modified, extending cache for key: {CacheKey}", cacheKey);
                return cached.Response;
            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var productList = JsonSerializer.Deserialize<ProductListResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new ProductListResponse();

            // Cache the response
            var etag = response.Headers.ETag?.Tag ?? string.Empty;
            var cacheEntry = new ProductListCacheEntry
            {
                Response = productList,
                ETag = etag,
                CachedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(DefaultCacheExpiry),
                Options = options
            };

            _listCache.AddOrUpdate(cacheKey, cacheEntry, (key, existing) => cacheEntry);

            _logger.LogInformation("Fetched {Count} products from API", productList.Products.Count);
            return productList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching products");
            OnError?.Invoke($"Failed to fetch products: {ex.Message}");

            // Return cached data if available, even if expired
            if (_listCache.TryGetValue(cacheKey, out var fallback))
            {
                _logger.LogWarning("Returning stale cached data due to error");
                return fallback.Response;
            }

            throw;
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    public async Task<Product?> GetProductAsync(string sku, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sku))
            return null;

        await _cacheLock.WaitAsync(cancellationToken);
        try
        {
            // Check cache first
            if (_productCache.TryGetValue(sku, out var cached) && cached.ExpiresAt > DateTime.UtcNow)
            {
                _logger.LogDebug("Returning cached product: {Sku}", sku);
                return cached.Product;
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"/v1/products/{Uri.EscapeDataString(sku)}");
            if (cached != null)
            {
                request.Headers.Add("If-None-Match", cached.ETag);
            }

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotModified && cached != null)
            {
                // Update cache expiry
                cached.ExpiresAt = DateTime.UtcNow.Add(DefaultCacheExpiry);
                _logger.LogDebug("Product not modified, extending cache: {Sku}", sku);
                return cached.Product;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Product not found: {Sku}", sku);
                return null;
            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var product = JsonSerializer.Deserialize<Product>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (product != null)
            {
                // Cache the product
                var etag = response.Headers.ETag?.Tag ?? string.Empty;
                var cacheEntry = new ProductCacheEntry
                {
                    Product = product,
                    ETag = etag,
                    CachedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.Add(DefaultCacheExpiry)
                };

                _productCache.AddOrUpdate(sku, cacheEntry, (key, existing) => cacheEntry);
                _logger.LogDebug("Cached product: {Sku}", sku);
            }

            return product;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product: {Sku}", sku);
            OnError?.Invoke($"Failed to fetch product {sku}: {ex.Message}");

            // Return cached data if available, even if expired
            if (_productCache.TryGetValue(sku, out var fallback))
            {
                _logger.LogWarning("Returning stale cached product due to error: {Sku}", sku);
                return fallback.Product;
            }

            throw;
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    public async Task<ProductChangesResponse> GetProductChangesAsync(string? sinceETag = null, DateTime? sinceTimestamp = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(sinceETag))
                queryParams.Add($"since={Uri.EscapeDataString(sinceETag)}");
            else if (sinceTimestamp.HasValue)
                queryParams.Add($"since={Uri.EscapeDataString(sinceTimestamp.Value.ToString("O"))}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var url = $"/v1/products/changes{queryString}";

            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var changes = JsonSerializer.Deserialize<ProductChangesResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new ProductChangesResponse();

            if (changes.Changes.Count > 0)
            {
                await ApplyProductChangesAsync(changes.Changes);
                OnProductsChanged?.Invoke(changes.Changes);
                _logger.LogInformation("Applied {Count} product changes", changes.Changes.Count);
            }

            _lastETag = changes.ETag;
            return changes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product changes");
            OnError?.Invoke($"Failed to fetch product changes: {ex.Message}");
            throw;
        }
    }

    public async Task InvalidateCacheAsync()
    {
        await _cacheLock.WaitAsync();
        try
        {
            _productCache.Clear();
            _listCache.Clear();
            _logger.LogInformation("Product cache cleared");
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    public async Task<bool> StartDeltaRefreshAsync(TimeSpan interval)
    {
        try
        {
            if (interval < TimeSpan.FromSeconds(1))
                interval = DefaultDeltaInterval;

            // Initial changes fetch to get current ETag
            await GetProductChangesAsync(_lastETag);

            _logger.LogInformation("Started delta refresh with interval: {Interval}", interval);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start delta refresh");
            OnError?.Invoke($"Failed to start delta refresh: {ex.Message}");
            return false;
        }
    }

    public Task StopDeltaRefreshAsync()
    {
        _logger.LogInformation("Stopped delta refresh");
        return Task.CompletedTask;
    }

    private async Task ApplyProductChangesAsync(List<ProductChange> changes)
    {
        await _cacheLock.WaitAsync();
        try
        {
            foreach (var change in changes)
            {
                switch (change.Operation.ToLowerInvariant())
                {
                    case "update":
                    case "create":
                        if (change.Product != null)
                        {
                            var cacheEntry = new ProductCacheEntry
                            {
                                Product = change.Product,
                                CachedAt = DateTime.UtcNow,
                                ExpiresAt = DateTime.UtcNow.Add(DefaultCacheExpiry)
                            };
                            _productCache.AddOrUpdate(change.Sku, cacheEntry, (key, existing) => cacheEntry);
                        }
                        break;

                    case "delete":
                        _productCache.TryRemove(change.Sku, out _);
                        break;
                }

                OnProductChanged?.Invoke(change);
            }

            // Invalidate list cache when products change
            _listCache.Clear();
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    private static string GenerateListCacheKey(ProductListOptions options)
    {
        var parts = new List<string>
        {
            $"page:{options.Page}",
            $"size:{options.PageSize}"
        };

        if (!string.IsNullOrEmpty(options.Category))
            parts.Add($"cat:{options.Category}");
        if (!string.IsNullOrEmpty(options.Search))
            parts.Add($"search:{options.Search}");
        if (options.Tags.Count > 0)
            parts.Add($"tags:{string.Join(",", options.Tags.OrderBy(t => t))}");

        return string.Join("|", parts);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _deltaRefreshTimer?.Dispose();
        _cacheLock?.Dispose();
        _httpClient?.Dispose();
        _disposed = true;
    }
}