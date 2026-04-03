using McpServer.Application.Interfaces;
using McpServer.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace McpServer.API.Controllers;

[ApiController]
[Route("message")]
public class McpController(IMcpProtocolService protocol) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> HandleMessage([FromBody] McpRequest request, CancellationToken ct)
    {
        var callerInfo = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var response = await protocol.HandleAsync(request, callerInfo, ct);
        return Ok(response);
    }
}