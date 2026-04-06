using System.Text.Json;
using McpServer.Application.Interfaces;
using McpServer.Domain.DTOs;
using McpServer.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace McpServer.Infrastructure.Tools.Grafana;

public class GrafanaAnalyzeTool(IClaudeClient claude, IServiceProvider sp) : IMcpTool
{
    public string Name => "grafana_analyze_alert";
    public string Description => "Analyzes a Grafana alert payload and sends an enriched message to Slack.";
    public string Category => "monitoring";
    public bool IsEnabled => true;

    public JsonElement InputSchema => JsonSerializer.SerializeToElement(new
    {
        type = "object",
        properties = new
        {
            alerts = new { type = "array", description = "Grafana alert list" }
        },
        required = new[] { "alerts" }
    });

    public async Task<ToolCallResponseDto> ExecuteAsync(JsonElement arguments, CancellationToken ct = default)
    {
        var payload = arguments.Deserialize<GrafanaAlertPayload>();
        if (payload is null)
            return ToolCallResponseDto.Failure("Failed to parse Grafana payload.");

        var processed = 0;

        using var scope = sp.CreateScope();
        var toolExecutor = scope.ServiceProvider.GetRequiredService<IToolExecutionService>();

        foreach (var alert in payload.Alerts)
        {
            alert.Labels.TryGetValue("app", out var appName);
            alert.Labels.TryGetValue("env", out var env);

            if (env != "PROD" || appName == null)
                continue;

            alert.Labels.TryGetValue("RequestPath", out var path);
            alert.Labels.TryGetValue("RequestMethod", out var method);
            alert.Labels.TryGetValue("Message", out var message);
            alert.Labels.TryGetValue("RequestBody", out var body);
            alert.Labels.TryGetValue("CallerSource", out var callerSource);
            alert.Labels.TryGetValue("CallerAppPage", out var callerAppPage);

            var analysis = await claude.CompleteAsync($$"""
                Service: {{appName}} ({{env}})
                Error: {{message}}
                Endpoint: {{method}} {{path}}
                Body: {{body}}
                Caller: {{callerSource}} / {{callerAppPage}}

                Write a short Slack alert message (max 3 lines, plain text, no markdown).
                State what likely went wrong and where.
                """, ct);

            var slackArgs = JsonSerializer.SerializeToElement(new
            {
                channel = "#alerts-prod",
                text = $"[{appName}] {analysis}"
            });

            await toolExecutor.ExecuteAsync(
                new ToolCallRequestDto("slack_send_message", slackArgs),
                callerInfo: "grafana_webhook",
                ct);

            processed++;
        }

        return ToolCallResponseDto.Success($"{processed} alert(s) processed.");
    }
}
