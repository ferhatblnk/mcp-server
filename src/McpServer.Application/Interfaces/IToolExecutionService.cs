using McpServer.Domain.DTOs;

namespace McpServer.Application.Interfaces;

public interface IToolExecutionService
{
    Task<ToolCallResponseDto> ExecuteAsync(ToolCallRequestDto request, string callerInfo, CancellationToken ct = default);
}
