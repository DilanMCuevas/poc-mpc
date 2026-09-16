namespace MCP.Server.Shared.Configuration;

public sealed class MongoDbOptions
{
    public const string SectionName = "MongoDb";

    public string Server { get; init; } = string.Empty;
    public int Port { get; init; }
    public string Database { get; init; } = string.Empty;
    public string User { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string AuthenticationDatabase { get; init; } = string.Empty;
}