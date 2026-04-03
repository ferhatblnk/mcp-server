using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using McpServer.Domain.DTOs;
using McpServer.Domain.Exceptions;
using McpServer.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace McpServer.Infrastructure.Http;

public class JiraHttpClient
{
    private readonly HttpClient _http;
    private readonly JiraOptions _options;
    private readonly ILogger<JiraHttpClient> _logger;

    public JiraHttpClient(HttpClient http, IOptions<JiraOptions> options, ILogger<JiraHttpClient> logger)
    {
        _options = options.Value;
        _logger = logger;
        _http = http;
        _http.BaseAddress = new Uri(_options.BaseUrl);
        _http.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                    System.Text.Encoding.UTF8.GetBytes($"{_options.Email}:{_options.ApiToken}")));
    }

    public async Task<JiraIssueResultDto> CreateIssueAsync(JiraCreateIssueDto dto, CancellationToken ct)
    {
        var payload = new
        {
            fields = new
            {
                project = new { key = _options.ProjectKey },
                summary = dto.Title,
                description = BuildAdfDescription(dto.Description),
                issuetype = new { name = "Bug" },
                priority = new { name = dto.Priority },
                labels = dto.Labels ?? Array.Empty<string>()
            }
        };

        _logger.LogDebug("Jira issue is being created. Title={Title}", dto.Title);

        var response = await _http.PostAsJsonAsync("/rest/api/3/issue", payload, ct);
        var body = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
            throw new ExternalServiceException("Jira", $"HTTP {(int)response.StatusCode}: {body}");

        var result = JsonSerializer.Deserialize<JiraCreateResponse>(body)
            ?? throw new ExternalServiceException("Jira", "No response received.");

        return new JiraIssueResultDto(result.Key, $"{_options.BaseUrl}/browse/{result.Key}");
    }

    public async Task<IEnumerable<JiraSprintDto>> GetActiveSprintsAsync(CancellationToken ct)
    {
        var response = await _http.GetAsync(
            $"/rest/agile/1.0/board/{_options.BoardId}/sprint?state=active", ct);

        if (!response.IsSuccessStatusCode)
            throw new ExternalServiceException("Jira", "The sprint list could not be retrieved.");

        var body = await response.Content.ReadFromJsonAsync<JiraSprintListResponse>(cancellationToken: ct);
        return body?.Values?.Select(s => new JiraSprintDto(s.Id.ToString(), s.Name))
               ?? Enumerable.Empty<JiraSprintDto>();
    }

    private static object BuildAdfDescription(string text) => new
    {
        type = "doc",
        version = 1,
        content = new[]
        {
            new
            {
                type    = "paragraph",
                content = new[] { new { type = "text", text } }
            }
        }
    };

    private record JiraCreateResponse([property: JsonPropertyName("key")] string Key);
    private record JiraSprintListResponse([property: JsonPropertyName("values")] List<JiraSprintItem>? Values);
    private record JiraSprintItem([property: JsonPropertyName("id")] int Id, [property: JsonPropertyName("name")] string Name);
}

public record JiraSprintDto(string Id, string Name);
