using System.Text.Json;
using McpServer.Domain.DTOs;
using McpServer.Domain.Exceptions;
using McpServer.Domain.Interfaces;
using McpServer.Infrastructure.Helpers;
using McpServer.Infrastructure.Http;
using Microsoft.Extensions.Logging;

namespace McpServer.Infrastructure.Tools.Jira;

public class JiraCreateIssueTool(JiraHttpClient client, ILogger<JiraCreateIssueTool> logger) : IMcpTool
{
    public string Name => "jira_create_issue";
    public string Description => "Jira opens new issues. It supports fields such as title, description, reporter, label, and sprint.";
    public string Category => "project-management";
    public bool IsEnabled => true;

    public JsonElement InputSchema => JiraHelpers.InputSchema();

    public async Task<ToolCallResponseDto> ExecuteAsync(JsonElement arguments, CancellationToken ct = default)
    {
        var dto = ParseArguments(arguments);
        var result = await client.CreateIssueAsync(dto, ct);

        logger.LogInformation("Jira issue created: {Key}", result.Key);

        return ToolCallResponseDto.Success($"Jira issue created: {result.Key}\nURL: {result.Url}");
    }

    private static JiraCreateIssueDto ParseArguments(JsonElement args)
    {
        if (!args.TryGetProperty("title", out var title) || string.IsNullOrWhiteSpace(title.GetString()))
            throw new ToolValidationException("'title' field is required.");

        if (!args.TryGetProperty("description", out var description) || string.IsNullOrWhiteSpace(description.GetString()))
            throw new ToolValidationException("'description' field is required.");

        string[]? labels = null;
        if (args.TryGetProperty("labels", out var labelsEl) && labelsEl.ValueKind == JsonValueKind.Array)
            labels = labelsEl.EnumerateArray().Select(x => x.GetString()!).ToArray();

        return new JiraCreateIssueDto(
            Title: title.GetString()!,
            Description: description.GetString()!,
            Reporter: args.TryGetProperty("reporter", out var r) ? r.GetString() : null,
            Labels: labels,
            SprintId: args.TryGetProperty("sprint_id", out var s) ? s.GetString() : null,
            Priority: args.TryGetProperty("priority", out var p) ? p.GetString() ?? "Medium" : "Medium"
        );
    }
}
