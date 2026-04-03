namespace McpServer.API.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseMcpMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<Middleware.GlobalExceptionMiddleware>();
        app.UseMiddleware<Middleware.ApiKeyMiddleware>();
        return app;
    }
}