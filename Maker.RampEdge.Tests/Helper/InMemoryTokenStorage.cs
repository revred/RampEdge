using Maker.RampEdge.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maker.RampEdge.Tests.Helper;

public sealed class InMemoryTokenStorage : ITokenStorage
{
    private readonly Dictionary<string, string> _store = new();

    public Task<string?> GetAsync(string key) =>
        Task.FromResult(_store.TryGetValue(key, out var v) ? v : null);

    public Task RemoveAsync(string key)
    {
        _store.Remove(key);
        return Task.CompletedTask;
    }

    public Task SetAsync(string key, string value)
    {
        _store[key] = value;
        return Task.CompletedTask;
    }
}
