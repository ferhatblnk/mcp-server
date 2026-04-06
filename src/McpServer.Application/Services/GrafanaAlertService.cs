using System.Text.Json;
using McpServer.Application.Interfaces;
using McpServer.Domain.DTOs;

namespace McpServer.Application.Services;

public class GrafanaAlertService(IToolExecutionService toolExecutor) : IGrafanaAlertService
{
    public async Task<bool> ProcessAlertAsync(GrafanaAlertPayload payload, CancellationToken ct = default)
    {
        var args = JsonSerializer.SerializeToElement(payload);

        await toolExecutor.ExecuteAsync(
            new ToolCallRequestDto("grafana_analyze_alert", args),
            callerInfo: "grafana_webhook",
            ct);

        return true;
    }
}
