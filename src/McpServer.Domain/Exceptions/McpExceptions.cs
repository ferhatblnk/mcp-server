namespace McpServer.Domain.Exceptions;

public class ToolNotFoundException(string toolName) : McpDomainException($"Tool '{toolName}' not found.", -32601) { }

public class ToolValidationException(string message) : McpDomainException(message, -32602) { }

public class ToolExecutionException(string toolName, string reason) : McpDomainException($"'{toolName}' error when is running: {reason}") { }

public class ExternalServiceException(string serviceName, string message) : McpDomainException($"{serviceName} service error: {message}")
{
    public string ServiceName { get; } = serviceName;
}
public abstract class McpDomainException(string message, int errorCode = -32000) : Exception(message)
{
    public int ErrorCode { get; } = errorCode;
}