using System.ComponentModel.DataAnnotations;

namespace McpServer.Infrastructure.Configuration;

public class JiraOptions
{
    public const string SectionName = "Jira";

    [Required] public string BaseUrl { get; init; } = "";
    [Required] public string Email { get; init; } = "";
    [Required] public string ApiToken { get; init; } = "";
    [Required] public string ProjectKey { get; init; } = "";
    public string BoardId { get; init; } = "1";
}

public class SlackOptions
{
    public const string SectionName = "Slack";

    [Required] public string WebhookUrl { get; init; } = "";
    public string DefaultChannel { get; init; } = "#general";
    public string BotUsername { get; init; } = "DevOps Bot";
    public string IconEmoji { get; init; } = ":robot_face:";
}

public class McpServerOptions
{
    public const string SectionName = "McpServer";

    public string Name { get; init; } = "DevOps MCP Server";
    public string Version { get; init; } = "1.0.0";
    public string ApiToken { get; init; } = "";
    public bool EnableSwagger { get; init; } = true;
}
