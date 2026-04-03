

using System.Text.Json;

namespace McpServer.Infrastructure.Helpers
{
    public static class JiraHelpers
    {
        public static JsonElement InputSchema()
        {
            return JsonSerializer.Deserialize<JsonElement>(
            """
                {
                  "type": "object",
                  "properties": {
                    "title": {
                      "type": "string",
                      "description": "Issue title (required)"
                    },
                    "description": {
                      "type": "string",
                      "description": "Detailed description of the issue (required)"
                    },
                    "reporter": {
                      "type": "string",
                      "description": "The username of the person who opened it in Jira"
                    },
                    "labels": {
                      "type": "array",
                      "items": {
                        "type": "string"
                      },
                      "description": "Label list"
                    },
                    "sprint_id": {
                      "type": "string",
                      "description": "Sprint ID"
                    },
                    "priority": {
                      "type": "string",
                      "enum": ["Low", "Medium", "High", "Critical"],
                      "default": "Medium"
                    }
                  },
                  "required": ["title", "description"]
                }
                """
            );
        }

    }
}