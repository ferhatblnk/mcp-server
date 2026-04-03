using System.Net.Http.Json;
using System.Text.Json;
using McpServer.Domain.DTOs;
using McpServer.Domain.Exceptions;
using McpServer.Domain.Interfaces;
using McpServer.Infrastructure.Configuration;
using McpServer.Infrastructure.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace McpServer.Infrastructure.Tools.Slack;

public class SlackSendMessageTool(HttpClient http, IOptions<SlackOptions> options, ILogger<SlackSendMessageTool> logger) : IMcpTool
{
    private readonly HttpClient _http = http;
    private readonly SlackOptions _options = options.Value;
    private readonly ILogger<SlackSendMessageTool> _logger = logger;

    public string Name => "slack_send_message";
    public string Description => "Sends a message to a Slack channel. Used for Grafana alerts and notifications.";
    public string Category => "messaging";
    public bool IsEnabled => true;

    public JsonElement InputSchema => SlackHelpers.InputSchema();

    public async Task<ToolCallResponseDto> ExecuteAsync(JsonElement arguments, CancellationToken ct = default)
    {
        if (!arguments.TryGetProperty("text", out var textEl) || string.IsNullOrWhiteSpace(textEl.GetString()))
            throw new ToolValidationException("'text' field is required.");

        var channel = arguments.TryGetProperty("channel", out var ch) ? ch.GetString() : null;

        var payload = new Dictionary<string, object>
        {
            ["channel"] = channel ?? _options.DefaultChannel,
            ["text"] = textEl.GetString()!,
            ["username"] = _options.BotUsername,
            ["icon_emoji"] = _options.IconEmoji
        };

        if (arguments.TryGetProperty("blocks", out var blocks))
            payload["blocks"] = blocks;

        _logger.LogDebug("Sending Slack message. Channel={Channel}", payload["channel"]);

        var response = await _http.PostAsJsonAsync(_options.WebhookUrl, payload, ct);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            throw new ExternalServiceException("Slack", $"HTTP {(int)response.StatusCode}: {body}");
        }

        return ToolCallResponseDto.Success(
            $"Message sent successfully. Channel: {payload["channel"]}");
    }
}