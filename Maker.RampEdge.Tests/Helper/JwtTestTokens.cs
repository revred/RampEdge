using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Maker.RampEdge.Tests.Helper;

internal static class JwtTestTokens
{
    public static string ValidWithEmail(int expSecondsFromNow, string email)
    {
        string header = Base64Url("""{"alg":"none","typ":"JWT"}""");
        var payloadObj = new
        {
            email,
            exp = DateTimeOffset.UtcNow.AddSeconds(expSecondsFromNow).ToUnixTimeSeconds(),
            nbf = DateTimeOffset.UtcNow.AddSeconds(-60).ToUnixTimeSeconds(),
            iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        string payload = Base64Url(JsonSerializer.Serialize(payloadObj));
        // unsigned token (your code only reads claims/exp)
        return $"{header}.{payload}.";
    }

    private static string Base64Url(string json)
    {
        var bytes = Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }
}
