using System.Text.Json;

namespace Agent.Integrations.Ollama;

public interface IOllamaService
{
    Task<string> ChatAsync(
        IReadOnlyList<OllamaMessage> messages,
        CancellationToken cancellationToken = default,
        JsonElement? responseFormat = null);
}
