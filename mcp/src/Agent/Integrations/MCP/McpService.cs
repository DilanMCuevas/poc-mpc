using Microsoft.Extensions.Options;
using ModelContextProtocol.Client;

namespace Agent.Integrations.MCP;

public sealed class McpService : IMcpService, IAsyncDisposable
{
    private readonly McpOptions _options;
    private McpClient? _client;

    public McpService(IOptions<McpOptions> options)
    {
        _options = options.Value;
    }

    public async Task<IReadOnlyList<McpToolInfo>> ListToolsAsync(
        CancellationToken cancellationToken = default)
    {
        var client = await GetClientAsync(cancellationToken);

        var tools = await client.ListToolsAsync(
            cancellationToken: cancellationToken);

        return tools
            .Select(tool => new McpToolInfo(
                tool.Name,
                tool.Description))
            .ToList();
    }
    public async Task<string> CallToolAsync(
    string toolName,
    IReadOnlyDictionary<string, object?> arguments,
    CancellationToken cancellationToken = default)
    {
        var client = await GetClientAsync(cancellationToken);

        var result = await client.CallToolAsync(
            toolName,
            arguments,
            cancellationToken: cancellationToken);

        return result.Content
            .OfType<ModelContextProtocol.Protocol.TextContentBlock>()
            .Select(content => content.Text)
            .FirstOrDefault()
            ?? string.Empty;
    }

    private async Task<McpClient> GetClientAsync(
        CancellationToken cancellationToken)
    {
        if (_client is not null)
        {
            return _client;
        }

        var transportOptions = new HttpClientTransportOptions
        {
            Name = "AcademicAgent",
            Endpoint = new Uri(_options.ServerUrl)
        };

        var transport = new HttpClientTransport(transportOptions);

        _client = await McpClient.CreateAsync(
            transport,
            cancellationToken: cancellationToken);

        return _client;
    }

    public async ValueTask DisposeAsync()
    {
        if (_client is not null)
        {
            await _client.DisposeAsync();
        }
    }
}