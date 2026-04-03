using Microsoft.AspNetCore.Mvc;

namespace McpServer.API.Controllers;

[ApiController]
[Route("sse")]
public class SseController : ControllerBase
{
    [HttpGet("ConnectAsync")]
    public async Task Connect(CancellationToken ct)
    {
        Response.Headers["Content-Type"] = "text/event-stream";
        Response.Headers["Cache-Control"] = "no-cache";
        Response.Headers["X-Accel-Buffering"] = "no";

        var endpoint = $"{Request.Scheme}://{Request.Host}/message";
        await Response.WriteAsync($"data: {{\"jsonrpc\":\"2.0\",\"method\":\"endpoint\",\"params\":{{\"uri\":\"{endpoint}\"}}}}\n\n", ct);
        await Response.Body.FlushAsync(ct);

        try { await Task.Delay(Timeout.Infinite, ct); }
        catch (OperationCanceledException) { }
    }
}
