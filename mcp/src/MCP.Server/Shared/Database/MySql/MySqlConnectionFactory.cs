using MCP.Server.Shared.Configuration;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace MCP.Server.Shared.Database.MySql;

public sealed class MySqlConnectionFactory
{
    private readonly MySqlOptions _options;

    public MySqlConnectionFactory(IOptions<MySqlOptions> options)
    {
        _options = options.Value;
    }

    public MySqlConnection CreateConnection()
    {
        var connectionString = new MySqlConnectionStringBuilder
        {
            Server = _options.Server,
            Port = (uint)_options.Port,
            Database = _options.Database,
            UserID = _options.User,
            Password = _options.Password
        }.ConnectionString;

        return new MySqlConnection(connectionString);
    }
}