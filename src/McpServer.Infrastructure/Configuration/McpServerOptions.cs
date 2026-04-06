namespace McpServer.Infrastructure.Configuration;

public class McpServerOptions
{
    public const string SectionName = "McpServer";

    public string Name { get; init; } = "DevOps MCP Server";
    public string Version { get; init; } = "1.0.0";
    public string ApiToken { get; init; } = "";
    public bool EnableSwagger { get; init; } = true;
}