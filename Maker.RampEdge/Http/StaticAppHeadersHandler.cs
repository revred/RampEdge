using Maker.RampEdge.Configuration;
using Microsoft.Extensions.Options;

namespace Maker.RampEdge.Http;

public sealed class StaticAppHeadersHandler : DelegatingHandler
{
    private readonly RampEdgeSettings _rampEdgeSettings;

    public StaticAppHeadersHandler(IOptions<RampEdgeSettings> rampEdgeSettings)
        => _rampEdgeSettings = rampEdgeSettings.Value;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        if (!request.Headers.Contains("BusinessUnitKey"))
            request.Headers.Add("BusinessUnitKey", _rampEdgeSettings.BusinessUnitKey);

        return await base.SendAsync(request, ct);
    }
}