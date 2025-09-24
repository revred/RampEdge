namespace Maker.RampEdge.Configuration;

public class RampEdgeSettings
{
    public const string SectionName = "RampEdge";
    public string BaseAddress { get; set; } = string.Empty;
    public string MakerProductBaseUrl { get; set; } = string.Empty;
    public string BusinessUnitKey { get; set; } = string.Empty;
}
