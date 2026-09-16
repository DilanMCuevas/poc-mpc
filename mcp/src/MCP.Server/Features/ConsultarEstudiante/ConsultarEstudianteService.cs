using Dapper;
using MCP.Server.Shared.Database.MySql;

namespace MCP.Server.Features.ConsultarEstudiante;

public sealed class ConsultarEstudianteService
{
    private readonly MySqlConnectionFactory _connectionFactory;

    private const string Sql = """
        SELECT
            e.id AS Id,
            e.boleta AS Boleta,
            e.nombre AS Nombre,
            e.correo AS Correo,
            e.telefono AS Telefono,
            pa.nombre AS ProgramaAcademico
        FROM estudiantes e
        INNER JOIN programas_academicos pa
            ON pa.id = e.programa_academico_id
        WHERE e.boleta = @Boleta;
        """;

    public ConsultarEstudianteService(
        MySqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ConsultarEstudianteResponse?> ExecuteAsync(
        string boleta,
        CancellationToken cancellationToken = default)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        var command = new CommandDefinition(
            commandText: Sql,
            parameters: new { Boleta = boleta },
            cancellationToken: cancellationToken
        );

        return await connection
            .QuerySingleOrDefaultAsync<ConsultarEstudianteResponse>(command);
    }
}