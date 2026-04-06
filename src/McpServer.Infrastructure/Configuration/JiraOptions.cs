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
