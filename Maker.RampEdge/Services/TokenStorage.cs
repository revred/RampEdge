using Maker.RampEdge.Services.Contracts;
using Microsoft.JSInterop;

namespace Maker.RampEdge.Services;

public class TokenStorage : ITokenStorage
{
    private readonly IJSRuntime _jsRuntime;
    public TokenStorage(IJSRuntime jSRuntime)
    {
        _jsRuntime = jSRuntime;
    }
    public async Task<string?> GetAsync(string key)
    {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
    }

    public async Task RemoveAsync(string key)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
    }

    public async Task SetAsync(string key, string value)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
    }
}
