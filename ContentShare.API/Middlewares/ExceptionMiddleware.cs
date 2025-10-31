using System.Net;
using System.Text.Json;

namespace ContentShare.API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        => (_next, _logger) = (next, logger);

    public async Task Invoke(HttpContext ctx)
    {
        try { await _next(ctx); }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Not found");
            ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
            ctx.Response.ContentType = "application/json";
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(new { error = ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error");
            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            ctx.Response.ContentType = "application/json";
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Unexpected error" }));
        }
    }
}
