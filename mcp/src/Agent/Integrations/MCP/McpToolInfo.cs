using System.Text.Json;
using System.Text.Json.Serialization;

namespace Agent.Integrations.MCP;

public sealed record McpToolInfo(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("inputSchema")] JsonElement InputSchema);
