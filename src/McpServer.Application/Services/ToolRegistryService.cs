using McpServer.Application.Interfaces;
using McpServer.Domain.DTOs;
using McpServer.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace McpServer.Application.Services;

public class ToolRegistryService(IEnumerable<IMcpTool> tools, ILogger<ToolRegistryService> logger) : IToolRegistryService
{
    private readonly IReadOnlyDictionary<string, IMcpTool> _tools = tools.Where(t => t.IsEnabled).ToDictionary(t => t.Name, StringComparer.OrdinalIgnoreCase);

    private readonly ILogger<ToolRegistryService> _logger = logger;

    public ToolRegistryService() : this([], null!)
    {
        _logger.LogInformation("Tool registry initialized. Number of registered tools: {Count}. Tools: {Names}",
            _tools.Count,
            string.Join(", ", _tools.Keys));
    }

    public IEnumerable<ToolDefinitionDto> GetAllTools()
    {
        return _tools.Values.Select(ToDto);
    }

    public IEnumerable<ToolDefinitionDto> GetToolsByCategory(string category)
    {
        return _tools.Values.Where(t => t.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).Select(ToDto);
    }

    public bool Exists(string toolName)
    {
        return _tools.ContainsKey(toolName);
    }

    public IMcpTool? Resolve(string toolName)
    {
        return _tools.GetValueOrDefault(toolName);
    }

    public static ToolDefinitionDto ToDto(IMcpTool tool)
    {
        return new(tool.Name, tool.Description, tool.InputSchema);
    }
}