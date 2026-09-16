using Dapper;
using MCP.Server.Shared.Database.MySql;

namespace MCP.Server.Features.ConsultarProfesor;

public sealed class ConsultarProfesorService
{
    private readonly MySqlConnectionFactory _connectionFactory;

    private const string Sql = """
        SELECT
            p.id AS Id,
            p.numero_empleado AS NumeroEmpleado,
            p.nombre AS Nombre,
            p.correo AS Correo,
            p.telefono AS Telefono
        FROM profesores p
        WHERE p.numero_empleado = @NumeroEmpleado;
        """;

    public ConsultarProfesorService(
        MySqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ConsultarProfesorResponse?> ExecuteAsync(
        string numeroEmpleado,
        CancellationToken cancellationToken = default)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        var command = new CommandDefinition(
            commandText: Sql,
            parameters: new { NumeroEmpleado = numeroEmpleado },
            cancellationToken: cancellationToken
        );

        return await connection
            .QuerySingleOrDefaultAsync<ConsultarProfesorResponse>(command);
    }
}
