using Maker.RampEdge;
using Maker.RampEdge.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Maker.RampEdge.Services;

public class ProductService
{
    private readonly IMakerClient _makerClient;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        [FromKeyedServices("Auth")] IMakerClient authClient,
        ILogger<ProductService> logger)
    {
        _makerClient = authClient;
        _logger = logger;
    }

    public async Task<ProductListResponse> ProductGroupsAsync()
    {
        try
        {
            return await _makerClient.ProductGroupsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during login.");
            return default!;
        }
    }
    public async Task<ProductListResponse> ProductsBySlugAsync(ProductRequest productRequest)
    {
        try
        {
            return await _makerClient.ProductsBySlugAsync(productRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during login.");
            return default!;
        }
    }
    public async Task AddProductsToCartAsync(AddCartRequest addCartRequest)
    {
        try
        {
            await _makerClient.AddProductsToCartAsync(addCartRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
    public async Task<bool> ClearUserCart()
    {
        try
        {
            await _makerClient.ClearCartAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return false;
        }
    }
    public async Task<GetCartReply> GetCartAsync()
    {
        try
        {
            return await _makerClient.GetCartAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return default!;
        }
    }
    public async Task<ProductDetailsReply> ProductDetailsAsync(ProductRequest body)
    {
        try
        {
            // Call your IMakerClient method
            return await _makerClient.ProductDetailsAsync(body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product details for {Slug}", body.Slug);
            return default!;
        }
    }
    public async Task RemoveProductFromTheCartAsync(RemoveProductRequest body)
    {
        try
        {
            await _makerClient.RemoveProductFromTheCartAsync(body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing product from cart: {Slug}", body.Slug);
        }
    }

}
