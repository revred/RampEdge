using Maker.RampEdge.Services.Contracts;
using System.Net;
using System.Net.Http.Headers;

namespace Maker.RampEdge.Http;

public sealed class BearerTokenHandler : DelegatingHandler
{
    private readonly IAuthenticationService _auth;
    private readonly Func<HttpRequestMessage, Task>? _onUnauthorized;

    public BearerTokenHandler(IAuthenticationService auth,
        Func<HttpRequestMessage, Task>? onUnauthorized = null)
    {
        _auth = auth;
        _onUnauthorized = onUnauthorized;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var path = request.RequestUri?.AbsolutePath ?? string.Empty;
        var isPublic = path.StartsWith("/api/public/", StringComparison.OrdinalIgnoreCase);

        if (!isPublic)
        {
            var exp = await _auth.GetAccessTokenExpiryAsync();
            if (!exp.HasValue || exp.Value <= DateTime.UtcNow.AddMinutes(1))
            {
                var ok = await _auth.RefreshTokenAsync();
                if (!ok)
                {
                    if (_onUnauthorized is not null)
                    {
                        await _onUnauthorized(request);
                        await _auth.LogoutAsync();
                    }
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized) { RequestMessage = request };
                }
            }

            var token = await _auth.GetAccessTokenAsync();
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, ct);

        if (response.StatusCode == HttpStatusCode.Unauthorized && _onUnauthorized is not null)
            await _onUnauthorized(request);

        return response;
    }
}