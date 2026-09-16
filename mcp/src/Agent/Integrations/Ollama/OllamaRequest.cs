using System.Text.Json;
using System.Text.Json.Serialization;

namespace Agent.Integrations.Ollama;

public sealed class OllamaRequest
{
    [JsonPropertyName("model")]
    public string Model { get; init; } = string.Empty;

    [JsonPropertyName("messages")]
    public IReadOnlyList<OllamaMessage> Messages { get; init; }
        = Array.Empty<OllamaMessage>();

    [JsonPropertyName("stream")]
    public bool Stream { get; init; }

    [JsonPropertyName("format")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonElement? Format { get; init; }

    [JsonPropertyName("think")]
    public bool Think { get; init; }
}

public sealed class OllamaMessage
{
    [JsonPropertyName("role")]
    public string Role { get; init; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; init; } = string.Empty;
}
