using Dapper;
using MCP.Server.Shared.Database.MySql;

namespace MCP.Server.Features.ConsultarHorarioEstudiante;

public sealed class ConsultarHorarioEstudianteService
{
    private readonly MySqlConnectionFactory _connectionFactory;

    private const string EstudianteSql = """
        SELECT
            e.id AS Id,
            e.boleta AS Boleta,
            e.nombre AS Nombre
        FROM estudiantes e
        WHERE e.boleta = @Boleta;
        """;

    private const string HorarioSql = """
        SELECT
            gm.id AS GrupoMateriaId,
            m.clave AS ClaveMateria,
            m.nombre AS Materia,
            g.clave_grupo AS ClaveGrupo,
            p.numero_empleado AS NumeroEmpleadoProfesor,
            p.nombre AS Profesor,
            h.dia AS Dia,
            h.hora_inicio AS HoraInicio,
            h.hora_fin AS HoraFin,
            s.numero AS Salon,
            s.tipo AS TipoSalon
        FROM inscripciones i
        INNER JOIN grupos_materia gm
            ON i.grupo_materia_id = gm.id
        INNER JOIN grupos g
            ON gm.grupo_id = g.id
        INNER JOIN materias m
            ON gm.materia_id = m.id
        INNER JOIN profesores p
            ON gm.profesor_id = p.id
        INNER JOIN horarios h
            ON h.grupo_materia_id = gm.id
        INNER JOIN salones s
            ON h.salon_id = s.id
        WHERE i.estudiante_id = @EstudianteId
            AND i.estado = 'ACTIVA'
            AND g.periodo = @Periodo
        ORDER BY h.dia, h.hora_inicio, h.id;
        """;

    public ConsultarHorarioEstudianteService(
        MySqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ConsultarHorarioEstudianteResponse?> ExecuteAsync(
        string boleta,
        string periodo,
        CancellationToken cancellationToken = default)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        var estudianteCommand = new CommandDefinition(
            commandText: EstudianteSql,
            parameters: new { Boleta = boleta },
            cancellationToken: cancellationToken
        );

        var estudiante = await connection
            .QuerySingleOrDefaultAsync<ConsultarHorarioEstudianteResponse>(estudianteCommand);

        if (estudiante is null)
        {
            return null;
        }

        var horarioCommand = new CommandDefinition(
            commandText: HorarioSql,
            parameters: new { EstudianteId = estudiante.Id, Periodo = periodo },
            cancellationToken: cancellationToken
        );

        var horario = await connection
            .QueryAsync<BloqueHorarioResponse>(horarioCommand);

        return new ConsultarHorarioEstudianteResponse
        {
            Id = estudiante.Id,
            Boleta = estudiante.Boleta,
            Nombre = estudiante.Nombre,
            Periodo = periodo,
            Horario = horario.ToList()
        };
    }
}
