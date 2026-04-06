namespace McpServer.Application.Interfaces;

public interface IClaudeClient
{
    Task<string> CompleteAsync(string prompt, CancellationToken ct = default);
}
