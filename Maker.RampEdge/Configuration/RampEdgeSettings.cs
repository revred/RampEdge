namespace Maker.RampEdge.Configuration;

public class RampEdgeSettings
{
    public const string SectionName = "RampEdge";
    public string BaseAddress { get; set; } = "https://maker-rest-api-e5c2djh7aafkace8.uksouth-01.azurewebsites.net";
    public string MakerProductBaseUrl { get; set; } = "https://black-bay-067463310.2.azurestaticapps.net";
    public string BusinessUnitKey { get; set; } = string.Empty;
}
