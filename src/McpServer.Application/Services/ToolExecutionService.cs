using McpServer.Application.Interfaces;
using McpServer.Domain.DTOs;
using McpServer.Domain.Entities;
using McpServer.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace McpServer.Application.Services;

public class ToolExecutionService(
    IEnumerable<Domain.Interfaces.IMcpTool> tools,
    ILogger<ToolExecutionService> logger) : IToolExecutionService
{
    private readonly IEnumerable<Domain.Interfaces.IMcpTool> _tools = tools;
    private readonly ILogger<ToolExecutionService> _logger = logger;

    private const int MaxRetries = 2;
    private const int TimeoutSeconds = 30;

    public async Task<ToolCallResponseDto> ExecuteAsync(ToolCallRequestDto request, string callerInfo, CancellationToken ct = default)
    {
        var tool = _tools.FirstOrDefault(t => t.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase))
                   ?? throw new ToolNotFoundException(request.Name);

        var execution = ToolExecution.Create(request.Name, callerInfo);

        _logger.LogInformation("Tool is running. Id={ExecutionId} Tool={ToolName} Caller={Caller}", execution.Id, execution.ToolName, execution.CallerInfo);

        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                cts.CancelAfter(TimeSpan.FromSeconds(TimeoutSeconds));

                var result = await tool.ExecuteAsync(request.Arguments, cts.Token);

                execution.Complete();

                _logger.LogInformation(
                    "The tool is complete. Id={ExecutionId} Duration={Duration}ms IsError={IsError}",
                    execution.Id, execution.Duration?.TotalMilliseconds, result.IsError);

                return result;
            }
            catch (OperationCanceledException) when (!ct.IsCancellationRequested)
            {
                _logger.LogWarning("Tool timeout. Id={ExecutionId} Attempt={Attempt}", execution.Id, attempt);
                if (attempt == MaxRetries)
                {
                    execution.Fail("Timeout");
                    throw new ToolExecutionException(request.Name, "The request has expired.");
                }
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "External service error. Id={ExecutionId} Service={Service}", execution.Id, ex.ServiceName);
                execution.Fail(ex.Message);
                return ToolCallResponseDto.Failure(ex.Message);
            }
            catch (ToolValidationException ex)
            {
                execution.Fail(ex.Message);
                return ToolCallResponseDto.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error. Id={ExecutionId} Attempt={Attempt}", execution.Id, attempt);
                if (attempt == MaxRetries)
                {
                    execution.Fail(ex.Message);
                    throw new ToolExecutionException(request.Name, ex.Message);
                }
                await Task.Delay(TimeSpan.FromMilliseconds(200 * attempt), ct);
            }
        }

        throw new ToolExecutionException(request.Name, "The maximum number of attempts has been reached.");
    }
}
