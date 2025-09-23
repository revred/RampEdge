using Microsoft.Extensions.Logging;

namespace Maker.RampEdge.Logging;

public interface ILogIds
{
    EventId ProductFetched { get; }
    EventId ProductCacheHit { get; }
    EventId ProductCacheMiss { get; }
    EventId ETagNotModified { get; }
    EventId DeltaApplied { get; }
    EventId DeltaRefreshStarted { get; }
    EventId DeltaRefreshStopped { get; }
    EventId CacheInvalidated { get; }
    EventId ApiError { get; }
    EventId HttpTimeout { get; }
    EventId BackoffTriggered { get; }
}

public sealed class LogIds : ILogIds
{
    public EventId ProductFetched      => new(1001, nameof(ProductFetched));
    public EventId ProductCacheHit     => new(1002, nameof(ProductCacheHit));
    public EventId ProductCacheMiss    => new(1003, nameof(ProductCacheMiss));
    public EventId ETagNotModified     => new(1004, nameof(ETagNotModified));
    public EventId DeltaApplied        => new(1005, nameof(DeltaApplied));
    public EventId DeltaRefreshStarted => new(1006, nameof(DeltaRefreshStarted));
    public EventId DeltaRefreshStopped => new(1007, nameof(DeltaRefreshStopped));
    public EventId CacheInvalidated    => new(1008, nameof(CacheInvalidated));
    public EventId ApiError            => new(1900, nameof(ApiError));
    public EventId HttpTimeout         => new(1901, nameof(HttpTimeout));
    public EventId BackoffTriggered    => new(1902, nameof(BackoffTriggered));
}