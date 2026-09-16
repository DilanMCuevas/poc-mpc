using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Agent.Integrations.Ollama;

public sealed class OllamaService : IOllamaService
{
    private readonly HttpClient _httpClient;
    private readonly OllamaOptions _options;

    public OllamaService(
        HttpClient httpClient,
        IOptions<OllamaOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<string> ChatAsync(
        IReadOnlyList<OllamaMessage> messages,
        CancellationToken cancellationToken = default,
        JsonElement? responseFormat = null)
    {
        var request = new OllamaRequest
        {
            Model = _options.Model,
            Stream = false,
            Messages = messages,
            Format = responseFormat,
            Think = false
        };

        using var response = await _httpClient.PostAsJsonAsync(
            "api/chat",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var ollamaResponse =
            await response.Content.ReadFromJsonAsync<OllamaResponse>(
                cancellationToken: cancellationToken);

        if (ollamaResponse?.Message is null
            || string.IsNullOrWhiteSpace(ollamaResponse.Message.Content))
        {
            throw new InvalidOperationException(
                "Ollama devolvió una respuesta vacía.");
        }

        return ollamaResponse.Message.Content;
    }
}
