using System.Text.Json;
using McpServer.Domain.DTOs;

namespace McpServer.Domain.Interfaces;

public interface IMcpTool
{
    string Name { get; }
    string Description { get; }
    string Category { get; }
    JsonElement InputSchema { get; }
    bool IsEnabled { get; }

    Task<ToolCallResponseDto> ExecuteAsync(JsonElement arguments, CancellationToken ct = default);
}
