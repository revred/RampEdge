namespace Maker.RampEdge.Configuration;

public class RampEdgeSettings
{
    public const string SectionName = "RampEdge";
    public string BaseAddress { get; set; } = string.Empty;
    public string ProductBaseUrl { get; set; } = string.Empty;
    public string BusinessUnitKey { get; set; } = string.Empty;
    public int TokenRefreshThresholdMinutes { get; set; } = 5;
    public bool EnableAutoTokenRefresh { get; set; } = true;
}