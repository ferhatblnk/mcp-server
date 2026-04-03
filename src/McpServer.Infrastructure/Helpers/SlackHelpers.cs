

using System.Text.Json;

namespace McpServer.Infrastructure.Helpers
{
  public static class SlackHelpers
  {
    public static JsonElement InputSchema()
    {
      return JsonSerializer.Deserialize<JsonElement>(
      """
       {
         "type": "object",
         "properties": {
           "channel": {
             "type": "string",
             "description": "Channel name, e.g. #alerts (optional, defaults to the configured channel)"
           },
           "text": {
             "type": "string",
             "description": "Message text to send (required)"
           },
           "blocks": {
             "type": "array",
             "description": "Slack Block Kit blocks (optional, for rich formatting)"
           }
         },
         "required": ["text"]
       }
       """
      );
    }
  }
}