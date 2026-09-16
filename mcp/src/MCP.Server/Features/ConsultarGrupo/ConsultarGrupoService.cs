using Dapper;
using MCP.Server.Shared.Database.MySql;

namespace MCP.Server.Features.ConsultarGrupo;

public sealed class ConsultarGrupoService
{
    private readonly MySqlConnectionFactory _connectionFactory;

    private const string GrupoSql = """
        SELECT
            g.id AS Id,
            g.clave_grupo AS ClaveGrupo,
            g.periodo AS Periodo,
            g.semestre AS Semestre
        FROM grupos g
        WHERE g.clave_grupo = @ClaveGrupo
            AND g.periodo = @Periodo;
        """;

    private const string MateriasSql = """
        SELECT
            gm.id AS GrupoMateriaId,
            m.clave AS Clave,
            m.nombre AS Nombre,
            m.creditos AS Creditos,
            p.numero_empleado AS NumeroEmpleadoProfesor,
            p.nombre AS Profesor,
            gm.cupo AS Cupo
        FROM grupos_materia gm
        INNER JOIN materias m
            ON gm.materia_id = m.id
        INNER JOIN profesores p
            ON gm.profesor_id = p.id
        WHERE gm.grupo_id = @GrupoId;
        """;

    public ConsultarGrupoService(
        MySqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ConsultarGrupoResponse?> ExecuteAsync(
        string claveGrupo,
        string periodo,
        CancellationToken cancellationToken = default)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        var grupoCommand = new CommandDefinition(
            commandText: GrupoSql,
            parameters: new { ClaveGrupo = claveGrupo, Periodo = periodo },
            cancellationToken: cancellationToken
        );

        var grupo = await connection
            .QuerySingleOrDefaultAsync<ConsultarGrupoResponse>(grupoCommand);

        if (grupo is null)
        {
            return null;
        }

        var materiasCommand = new CommandDefinition(
            commandText: MateriasSql,
            parameters: new { GrupoId = grupo.Id },
            cancellationToken: cancellationToken
        );

        var materias = await connection
            .QueryAsync<MateriaGrupoResponse>(materiasCommand);

        return new ConsultarGrupoResponse
        {
            Id = grupo.Id,
            ClaveGrupo = grupo.ClaveGrupo,
            Periodo = grupo.Periodo,
            Semestre = grupo.Semestre,
            Materias = materias.ToList()
        };
    }
}
