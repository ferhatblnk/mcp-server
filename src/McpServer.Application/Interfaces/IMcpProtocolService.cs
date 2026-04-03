
using McpServer.Domain.DTOs;

namespace McpServer.Application.Interfaces;

public interface IMcpProtocolService
{
    Task<McpResponse> HandleAsync(McpRequest request, string callerInfo, CancellationToken ct = default);
}
