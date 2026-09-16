namespace Agent.Integrations.MCP;

public sealed class McpOptions
{
    public const string SectionName = "Mcp";

    public string ServerUrl { get; init; } = string.Empty;
}