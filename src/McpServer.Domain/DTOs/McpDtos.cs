using System.Text.Json;
using System.Text.Json.Serialization;

namespace McpServer.Domain.DTOs;


public record McpRequest(
    [property: JsonPropertyName("jsonrpc")] string JsonRpc,
    [property: JsonPropertyName("id")] JsonElement? Id,
    [property: JsonPropertyName("method")] string Method,
    [property: JsonPropertyName("params")] JsonElement? Params
);

public record McpResponse(
    [property: JsonPropertyName("jsonrpc")] string JsonRpc,
    [property: JsonPropertyName("id")] JsonElement? Id,
    [property: JsonPropertyName("result")] object? Result = null,
    [property: JsonPropertyName("error")] McpError? Error = null
)
{
    public static McpResponse Ok(JsonElement? id, object result) =>
        new("2.0", id, result);

    public static McpResponse Fail(JsonElement? id, int code, string message) =>
        new("2.0", id, Error: new McpError(code, message));
}

public record McpError(
    [property: JsonPropertyName("code")] int Code,
    [property: JsonPropertyName("message")] string Message
);


public record ToolDefinitionDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("inputSchema")] JsonElement InputSchema
);

public record ToolListResponseDto(
    [property: JsonPropertyName("tools")] IEnumerable<ToolDefinitionDto> Tools
);


public record ToolCallRequestDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("arguments")] JsonElement Arguments
);

public record ToolCallResponseDto(
    [property: JsonPropertyName("content")] IEnumerable<ToolContentDto> Content,
    [property: JsonPropertyName("isError")] bool IsError
)
{
    public static ToolCallResponseDto Success(string text) =>
        new([new ToolContentDto("text", text)], false);

    public static ToolCallResponseDto Failure(string error) =>
        new([new ToolContentDto("text", error)], true);
}

public record ToolContentDto(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("text")] string Text
);


public record InitializeResponseDto([property: JsonPropertyName("protocolVersion")] string ProtocolVersion, [property: JsonPropertyName("capabilities")] ServerCapabilitiesDto Capabilities, [property: JsonPropertyName("serverInfo")] ServerInfoDto ServerInfo);

public record ServerCapabilitiesDto([property: JsonPropertyName("tools")] object Tools = null!)
{
    public static readonly ServerCapabilitiesDto Default = new(new { });
}

public record ServerInfoDto([property: JsonPropertyName("name")] string Name, [property: JsonPropertyName("version")] string Version);


public record JiraCreateIssueDto(
    string Title,
    string Description,
    string? Reporter,
    string[]? Labels,
    string? SprintId,
    string Priority = "Medium"
);

public record JiraIssueResultDto(
    string Key,
    string Url
);


public record SlackMessageDto(
    string Channel,
    string Text,
    string? Username = null,
    string? IconEmoji = null
);
