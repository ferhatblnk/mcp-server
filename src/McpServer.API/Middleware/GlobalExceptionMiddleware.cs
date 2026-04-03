using System.Text.Json;
using McpServer.Domain.Exceptions;

namespace McpServer.API.Middleware;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Invoke error. Path={Path}", ctx.Request.Path);
            await WriteErrorAsync(ctx, ex);
        }
    }

    private static Task WriteErrorAsync(HttpContext ctx, Exception ex)
    {
        ctx.Response.ContentType = "application/json";

        var (status, code, message) = ex switch
        {
            ToolNotFoundException e => (404, e.ErrorCode, e.Message),
            ToolValidationException e => (400, e.ErrorCode, e.Message),
            ToolExecutionException e => (500, e.ErrorCode, e.Message),
            McpDomainException e => (500, e.ErrorCode, e.Message),
            _ => (500, -32000, "Server error")
        };

        ctx.Response.StatusCode = status;

        return ctx.Response.WriteAsync(JsonSerializer.Serialize(new
        {
            jsonrpc = "2.0",
            id = (object?)null,
            error = new { code, message }
        }));
    }
}
