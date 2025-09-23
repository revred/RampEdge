using System.Text.Json.Serialization;

namespace Maker.RampEdge.Models;

public class Product
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("sku")]
    public string Sku { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "USD";

    [JsonPropertyName("availability")]
    public ProductAvailability Availability { get; set; } = new();

    [JsonPropertyName("specifications")]
    public List<ProductSpecification> Specifications { get; set; } = new();

    [JsonPropertyName("media")]
    public List<ProductMedia> Media { get; set; } = new();

    [JsonPropertyName("moq")]
    public int MinimumOrderQuantity { get; set; } = 1;

    [JsonPropertyName("inStock")]
    public bool InStock { get; set; } = true;

    [JsonPropertyName("leadTime")]
    public string LeadTime { get; set; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}

public class ProductAvailability
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = "available";

    [JsonPropertyName("stockLevel")]
    public int StockLevel { get; set; }

    [JsonPropertyName("leadTimeDays")]
    public int LeadTimeDays { get; set; }

    [JsonPropertyName("backOrderAllowed")]
    public bool BackOrderAllowed { get; set; }
}

public class ProductSpecification
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("unit")]
    public string Unit { get; set; } = string.Empty;

    [JsonPropertyName("displayOrder")]
    public int DisplayOrder { get; set; }
}

public class ProductMedia
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("kind")]
    public string Kind { get; set; } = "image";

    [JsonPropertyName("alt")]
    public string Alt { get; set; } = string.Empty;

    [JsonPropertyName("caption")]
    public string Caption { get; set; } = string.Empty;

    [JsonPropertyName("displayOrder")]
    public int DisplayOrder { get; set; }
}

public class ProductListResponse
{
    [JsonPropertyName("products")]
    public List<Product> Products { get; set; } = new();

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    [JsonPropertyName("page")]
    public int Page { get; set; } = 1;

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; } = 24;

    [JsonPropertyName("hasNextPage")]
    public bool HasNextPage { get; set; }

    [JsonPropertyName("hasPreviousPage")]
    public bool HasPreviousPage { get; set; }
}

public class ProductChangesResponse
{
    [JsonPropertyName("changes")]
    public List<ProductChange> Changes { get; set; } = new();

    [JsonPropertyName("etag")]
    public string ETag { get; set; } = string.Empty;

    [JsonPropertyName("hasMore")]
    public bool HasMore { get; set; }
}

public class ProductChange
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("sku")]
    public string Sku { get; set; } = string.Empty;

    [JsonPropertyName("operation")]
    public string Operation { get; set; } = "update";

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("product")]
    public Product? Product { get; set; }
}

public class ProductListOptions
{
    public string? Category { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 24;
    public string? Search { get; set; }
    public List<string> Tags { get; set; } = new();
}

public class ProductCacheEntry
{
    public Product Product { get; set; } = new();
    public string ETag { get; set; } = string.Empty;
    public DateTime CachedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}

public class ProductListCacheEntry
{
    public ProductListResponse Response { get; set; } = new();
    public string ETag { get; set; } = string.Empty;
    public DateTime CachedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public ProductListOptions Options { get; set; } = new();
}