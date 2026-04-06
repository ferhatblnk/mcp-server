using System.Net.Http.Json;
using System.Text.Json;
using McpServer.Application.Interfaces;
using McpServer.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace McpServer.Infrastructure.Http;

public class ClaudeHttpClient(HttpClient http, IOptions<ClaudeOptions> options) : IClaudeClient
{
    private readonly ClaudeOptions _options = options.Value;

    public async Task<string> CompleteAsync(string prompt, CancellationToken ct = default)
    {
        var body = new
        {
            model = "claude-sonnet-4-20250514",
            max_tokens = 300,
            messages = new[] { new { role = "user", content = prompt } }
        };

        var response = await http.PostAsJsonAsync("v1/messages", body, ct);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
        return result.GetProperty("content")[0].GetProperty("text").GetString() ?? string.Empty;
    }
}
