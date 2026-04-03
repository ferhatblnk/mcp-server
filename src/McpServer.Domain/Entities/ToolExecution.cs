namespace McpServer.Domain.Entities;

public class ToolExecution
{
    public Guid Id { get; private set; }
    public string? ToolName { get; set; }
    public string? CallerInfo { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public ToolExecutionStatus Status { get; set; }
    public string? ErrorMessage { get; set; }

    public static ToolExecution Create(string toolName, string callerInfo) => new()
    {
        Id = Guid.NewGuid(),
        ToolName = toolName,
        CallerInfo = callerInfo,
        StartedAt = DateTime.UtcNow,
        Status = ToolExecutionStatus.Running
    };

    public void Complete()
    {
        CompletedAt = DateTime.UtcNow;
        Status = ToolExecutionStatus.Succeeded;
    }

    public void Fail(string error)
    {
        CompletedAt = DateTime.UtcNow;
        Status = ToolExecutionStatus.Failed;
        ErrorMessage = error;
    }

    public TimeSpan? Duration => CompletedAt.HasValue
        ? CompletedAt.Value - StartedAt
        : null;
}

public enum ToolExecutionStatus { Running, Succeeded, Failed }
