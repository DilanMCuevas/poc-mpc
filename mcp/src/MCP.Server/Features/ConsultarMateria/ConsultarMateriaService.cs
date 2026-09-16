using Dapper;
using MCP.Server.Shared.Database.MySql;

namespace MCP.Server.Features.ConsultarMateria;

public sealed class ConsultarMateriaService
{
    private readonly MySqlConnectionFactory _connectionFactory;

    private const string Sql = """
        SELECT
            m.id AS Id,
            m.clave AS Clave,
            m.nombre AS Nombre,
            m.creditos AS Creditos,
            m.semestre AS Semestre,
            pa.nombre AS ProgramaAcademico
        FROM materias m
        INNER JOIN programas_academicos pa
            ON m.programa_academico_id = pa.id
        WHERE m.clave = @Clave;
        """;

    public ConsultarMateriaService(
        MySqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ConsultarMateriaResponse?> ExecuteAsync(
        string clave,
        CancellationToken cancellationToken = default)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        var command = new CommandDefinition(
            commandText: Sql,
            parameters: new { Clave = clave },
            cancellationToken: cancellationToken
        );

        return await connection
            .QuerySingleOrDefaultAsync<ConsultarMateriaResponse>(command);
    }
}
