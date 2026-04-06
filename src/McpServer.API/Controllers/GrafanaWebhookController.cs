using McpServer.Application.Interfaces;
using McpServer.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace McpServer.API.Controllers;

[ApiController]
[Route("api/grafana")]
public class GrafanaWebhookController(IGrafanaAlertService alertService) : ControllerBase
{
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook([FromBody] GrafanaAlertPayload payload, CancellationToken ct)
    {
        await alertService.ProcessAlertAsync(payload, ct);
        return Ok();
    }
}
