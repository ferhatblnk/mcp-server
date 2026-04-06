using McpServer.Domain.DTOs;

namespace McpServer.Application.Interfaces;

public interface IGrafanaAlertService
{
    Task<bool> ProcessAlertAsync(GrafanaAlertPayload payload, CancellationToken ct = default);
}
