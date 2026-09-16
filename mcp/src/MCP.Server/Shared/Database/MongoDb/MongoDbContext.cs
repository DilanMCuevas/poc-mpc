using MCP.Server.Shared.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MCP.Server.Shared.Database.MongoDb;

public sealed class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbOptions> options)
    {
        var settings = options.Value;

        var credential = MongoCredential.CreateCredential(
            settings.AuthenticationDatabase,
            settings.User,
            settings.Password);

        var clientSettings = new MongoClientSettings
        {
            Server = new MongoServerAddress(
                settings.Server,
                settings.Port),
            Credential = credential
        };

        var client = new MongoClient(clientSettings);

        _database = client.GetDatabase(settings.Database);
    }

    public IMongoDatabase Database => _database;

    public async Task PingAsync(
        CancellationToken cancellationToken = default)
    {
        await _database.RunCommandAsync<BsonDocument>(
            new BsonDocument("ping", 1),
            cancellationToken: cancellationToken);
    }
}