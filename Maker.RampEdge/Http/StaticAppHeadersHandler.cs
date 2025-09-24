using Maker.RampEdge.Configuration;
using Maker.RampEdge.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maker.RampEdge.Http;

// Adds Apikey + BusinessUnitKey to every request
public sealed class StaticAppHeadersHandler : DelegatingHandler
{
    private readonly RampEdgeSettings _makerSettings;
    public StaticAppHeadersHandler(IOptions<RampEdgeSettings> makerSettings)
        => _makerSettings = makerSettings.Value;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        if (!request.Headers.Contains("BusinessUnitKey"))
            request.Headers.Add("BusinessUnitKey", _makerSettings.BusinessUnitKey);

        return await base.SendAsync(request, ct);
    }
}

