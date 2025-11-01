using Microsoft.Extensions.Caching.Memory;
using Serilog;
using System.Security.Cryptography;
using System.Text;

namespace ContentShare.API.Middlewares;

public class HmacMiddleware : IMiddleware
{
    private const string SignatureHeader = "X-HMAC-SIGNATURE";
    private const string TimestampHeader = "X-HMAC-TIMESTAMP";
    private const string NonceHeader = "X-HMAC-NONCE";

    private readonly string _secretKey;
    private readonly IMemoryCache _memoryCache;
    private readonly TimeSpan _allowedTimeDrift = TimeSpan.FromMinutes(5);

    public HmacMiddleware(IConfiguration configuration, IMemoryCache memoryCache)
    {
        _secretKey = configuration["HmacOptions:SecretKey"]
            ?? throw new ArgumentNullException("SecretKey is missing in configuration");
        _memoryCache = memoryCache;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var endpoint = context.GetEndpoint();
        var requiresHmac = endpoint?.Metadata.GetMetadata<UseHmacAttribute>() != null;

        if (!requiresHmac)
        {
            await next(context);
            return;
        }
        if (!context.Request.Headers.TryGetValue(SignatureHeader, out var receivedSignature) ||
            !context.Request.Headers.TryGetValue(TimestampHeader, out var timestampHeader) ||
            !context.Request.Headers.TryGetValue(NonceHeader, out var nonceHeader))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Missing HMAC signature, timestamp, or nonce header");
            Log.Error("Missing HMAC signature, timestamp, or nonce header");
            return;
        }

        if (!long.TryParse(timestampHeader, out var requestUnixTimestamp))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Invalid timestamp format");
            Log.Error("Invalid timestamp format");
            return;
        }

        var requestTime = DateTimeOffset.FromUnixTimeSeconds(requestUnixTimestamp);
        var now = DateTimeOffset.UtcNow;

        if (Math.Abs((now - requestTime).TotalSeconds) > _allowedTimeDrift.TotalSeconds)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Request timestamp is outside the allowed window");
            Log.Error("Request timestamp is outside the allowed window");
            return;
        }

        var method = context.Request.Method;
        var path = context.Request.Path.Value ?? "";
        var nonce = nonceHeader.ToString();

        if (_memoryCache.TryGetValue($"nonce_{nonce}", out _))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Replay attack detected: Nonce already used");
            Log.Error("Replay attack detected: Nonce already used");
            return;
        }

        string message = $"{method}.{timestampHeader}.{nonce}";

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
        var computedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();

        var isValid = CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computedSignature),
            Encoding.UTF8.GetBytes(receivedSignature)
        );

        if (!isValid)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid HMAC signature");
            Log.Error("Invalid HMAC signature");
            return;
        }

        if (_memoryCache.TryGetValue(receivedSignature, out _))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Replay attack detected: Signature already used");
            Log.Error("Replay attack detected: Signature already used");
            return;
        }

        _memoryCache.Set(receivedSignature, true, _allowedTimeDrift);
        _memoryCache.Set($"nonce_{nonce}", true, _allowedTimeDrift);

        await next(context);
    }
}
