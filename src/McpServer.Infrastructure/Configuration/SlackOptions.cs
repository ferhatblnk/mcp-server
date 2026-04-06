

using System.ComponentModel.DataAnnotations;

namespace McpServer.Infrastructure.Configuration;

public class SlackOptions
{
    public const string SectionName = "Slack";

    [Required] public string WebhookUrl { get; init; } = "";
    public string DefaultChannel { get; init; } = "#general";
    public string BotUsername { get; init; } = "DevOps Bot";
    public string IconEmoji { get; init; } = ":robot_face:";
}