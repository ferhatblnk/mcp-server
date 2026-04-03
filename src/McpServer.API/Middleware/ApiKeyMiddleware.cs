using McpServer.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace McpServer.API.Middleware;

public class ApiKeyMiddleware(RequestDelegate nextDelegate, IOptions<McpServerOptions> options)
{
    private static readonly HashSet<string> _openPaths = new(StringComparer.OrdinalIgnoreCase)
    {
        "/health", "/sse"
    };

    public async Task InvokeAsync(HttpContext ctx)
    {
        // if (string.IsNullOrEmpty(options.Value.ApiKey) || _openPaths.Contains(ctx.Request.Path))
        // {
        //     await nextDelegate(ctx);
        //     return;
        // }

        // if (!ctx.Request.Headers.TryGetValue("X-Api-Key", out var key) || key != options.Value.ApiKey)
        // {
        //     ctx.Response.StatusCode = 401;
        //     ctx.Response.ContentType = "application/json";
        //     await ctx.Response.WriteAsync("{\"error\":\"Invalid or incomplete API Key.\"}");
        //     return;
        // }

        await nextDelegate(ctx);
    }
}