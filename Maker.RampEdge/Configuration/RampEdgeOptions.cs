using System;

namespace Maker.RampEdge.Configuration;

public sealed class RampEdgeOptions
{
    /// <summary>Base URL of the RampEdge/MAK3R Product API, e.g., https://api.example.com/</summary>
    public string ApiBaseUrl { get; set; } = "https://api.example.com/";

    /// <summary>Default page size for product lists.</summary>
    public int DefaultPageSize { get; set; } = 24;

    /// <summary>Delta polling interval in seconds. WP01 target is <= 2s.</summary>
    public int DeltaPollSeconds { get; set; } = 2;

    /// <summary>Maximum backoff seconds when server signals overload (429/5xx).</summary>
    public int MaxBackoffSeconds { get; set; } = 30;

    /// <summary>Cache expiry time in minutes.</summary>
    public int CacheExpiryMinutes { get; set; } = 15;

    /// <summary>HTTP client timeout in seconds.</summary>
    public int HttpTimeoutSeconds { get; set; } = 10;
}