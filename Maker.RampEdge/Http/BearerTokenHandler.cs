using Maker.RampEdge.Services.Contracts;
using System.Net;
using System.Net.Http.Headers;

namespace Maker.RampEdge.Http;

// Attaches/refreshes Bearer token (use on API client only)
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
                        await _onUnauthorized(request); // notify caller
                        await _auth.LogoutAsync();
                    }
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized) { RequestMessage = request };
                }
            }

            var token = await _auth.GetAccessTokenAsync();
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // send to server
        var response = await base.SendAsync(request, ct);

        // post-response: if API says 401 (e.g., server invalidated token), notify
        if (response.StatusCode == HttpStatusCode.Unauthorized && _onUnauthorized is not null)
            await _onUnauthorized(request);

        return response;
    }
}
