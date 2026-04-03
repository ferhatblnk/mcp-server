using System.Text.Json;
using McpServer.Application.Interfaces;
using McpServer.Domain.DTOs;
using McpServer.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace McpServer.Application.Services;

public class McpProtocolService(
    IToolRegistryService registry,
    IToolExecutionService executor,
    IConfiguration config,
    ILogger<McpProtocolService> logger) : IMcpProtocolService
{

    public async Task<McpResponse> HandleAsync(McpRequest request, string callerInfo, CancellationToken ct = default)
    {
        logger.LogDebug("MCP request received. Method={Method}", request.Method);

        try
        {
            var result = request.Method switch
            {
                "initialize" => (object)BuildInitializeResponse(),
                "tools/list" => BuildToolListResponse(),
                "tools/call" => await HandleToolCallAsync(request, callerInfo, ct),
                "ping" => new { pong = true },
                _ => throw new McpDomainExceptionUnknownMethod(request.Method)
            };

            return McpResponse.Ok(request.Id, result);
        }
        catch (McpDomainException ex)
        {
            logger.LogWarning("Domain error. Method={Method} Error={Error}", request.Method, ex.Message);
            return McpResponse.Fail(request.Id, ex.ErrorCode, ex.Message);
        }
    }

    private InitializeResponseDto BuildInitializeResponse() => new(
        ProtocolVersion: "2024-11-05",
        Capabilities: ServerCapabilitiesDto.Default,
        ServerInfo: new ServerInfoDto(
            config["McpServer:Name"] ?? "DevOps MCP Server",
            config["McpServer:Version"] ?? "1.0.0"
        )
    );

    private ToolListResponseDto BuildToolListResponse()
    {
        return new(registry.GetAllTools());
    }


    private async Task<ToolCallResponseDto> HandleToolCallAsync(McpRequest request, string callerInfo, CancellationToken ct)
    {
        if (request.Params is null)
            throw new ToolValidationException("tools/call parameters are required");

        var callRequest = JsonSerializer.Deserialize<ToolCallRequestDto>(
            request.Params.Value.GetRawText(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new ToolValidationException("Invalid tool call payload.");

        return await executor.ExecuteAsync(callRequest, callerInfo, ct);
    }
}

internal class McpDomainExceptionUnknownMethod(string method) : McpDomainException($"Unexpected error: '{method}'", -32601)
{
}
