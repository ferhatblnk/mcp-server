

namespace McpServer.Domain.DTOs;

public class GrafanaAlertPayload
{
    public List<GrafanaAlert> Alerts { get; set; } = [];
}

public class GrafanaAlert
{
    public Dictionary<string, string> Labels { get; set; } = [];
    public DateTimeOffset StartsAt { get; set; }
    public string Status { get; set; }
}