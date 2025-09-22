using System.Threading.Tasks;

namespace Maker.RampEdge.Services.Contracts;

public interface ITokenStorage
{
    Task SetAsync(string key, string? value);
    Task<string?> GetAsync(string key);
    Task RemoveAsync(string key);
}
