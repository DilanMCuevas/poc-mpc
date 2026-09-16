using System.Text.Json.Serialization;

namespace Agent.Integrations.Ollama;

public sealed class OllamaResponse
{
    [JsonPropertyName("model")]
    public string Model { get; init; } = string.Empty;

    [JsonPropertyName("message")]
    public OllamaMessage Message { get; init; } = new();

    [JsonPropertyName("done")]
    public bool Done { get; init; }
}