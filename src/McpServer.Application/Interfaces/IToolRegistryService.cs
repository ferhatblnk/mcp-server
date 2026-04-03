using McpServer.Domain.DTOs;

namespace McpServer.Application.Interfaces;

public interface IToolRegistryService
{
    IEnumerable<ToolDefinitionDto> GetAllTools();
    IEnumerable<ToolDefinitionDto> GetToolsByCategory(string category);
    bool Exists(string toolName);
}


