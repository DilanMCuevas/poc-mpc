using Dapper;
using MCP.Server.Features.ConsultarEvaluaciones;
using MCP.Server.Features.ConsultarEvaluaciones.Responses;
using MCP.Server.Shared.Database.MySql;

namespace MCP.Server.Features.ConsultarCargaAcademica;

public sealed class ConsultarCargaAcademicaService
{
    private readonly MySqlConnectionFactory _connectionFactory;
    private readonly ConsultarEvaluacionesService _evaluacionesService;

    private const string EstudianteSql = """
        SELECT
            e.id AS Id,
            e.boleta AS Boleta,
            e.nombre AS Nombre
        FROM estudiantes e
        WHERE e.boleta = @Boleta;
        """;

    private const string MateriasSql = """
        SELECT
            gm.id AS GrupoMateriaId,
            m.id AS MateriaId,
            m.clave AS ClaveMateria,
            m.nombre AS Materia,
            m.creditos AS Creditos,
            g.clave_grupo AS ClaveGrupo,
            p.numero_empleado AS NumeroEmpleadoProfesor,
            p.nombre AS Profesor,
            gm.cupo AS Cupo
        FROM inscripciones i
        INNER JOIN grupos_materia gm
            ON i.grupo_materia_id = gm.id
        INNER JOIN grupos g
            ON gm.grupo_id = g.id
        INNER JOIN materias m
            ON gm.materia_id = m.id
        INNER JOIN profesores p
            ON gm.profesor_id = p.id
        WHERE i.estudiante_id = @EstudianteId
            AND i.estado = 'ACTIVA'
            AND g.periodo = @Periodo
        ORDER BY m.clave, gm.id;
        """;

    public ConsultarCargaAcademicaService(
        MySqlConnectionFactory connectionFactory,
        ConsultarEvaluacionesService evaluacionesService)
    {
        _connectionFactory = connectionFactory;
        _evaluacionesService = evaluacionesService;
    }

    public async Task<ConsultarCargaAcademicaResponse?> ExecuteAsync(
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
            .QuerySingleOrDefaultAsync<ConsultarCargaAcademicaResponse>(estudianteCommand);

        if (estudiante is null)
        {
            return null;
        }

        var materiasCommand = new CommandDefinition(
            commandText: MateriasSql,
            parameters: new { EstudianteId = estudiante.Id, Periodo = periodo },
            cancellationToken: cancellationToken
        );

        var materias = (await connection
            .QueryAsync<MateriaCargaAcademicaResponse>(materiasCommand)).ToList();

        IReadOnlyList<ConsultarEvaluacionesResponse> evaluaciones = [];

        if (materias.Count > 0)
        {
            evaluaciones = await _evaluacionesService.ExecuteAsync(
                estudiante.Id,
                periodo,
                cancellationToken);
        }

        // La respuesta representa un intento por asignación: se toma el más alto.
        var evaluacionesPorGrupoMateria = evaluaciones
            .GroupBy(evaluacion => evaluacion.GrupoMateriaId)
            .ToDictionary(
                grupo => grupo.Key,
                grupo => grupo.OrderByDescending(evaluacion => evaluacion.Intento).First());

        return new ConsultarCargaAcademicaResponse
        {
            Id = estudiante.Id,
            Boleta = estudiante.Boleta,
            Nombre = estudiante.Nombre,
            Periodo = periodo,
            Materias = materias.Select(materia =>
            {
                evaluacionesPorGrupoMateria.TryGetValue(
                    materia.GrupoMateriaId,
                    out var evaluacion);

                return new MateriaCargaAcademicaResponse
                {
                    GrupoMateriaId = materia.GrupoMateriaId,
                    MateriaId = materia.MateriaId,
                    ClaveMateria = materia.ClaveMateria,
                    Materia = materia.Materia,
                    Creditos = materia.Creditos,
                    ClaveGrupo = materia.ClaveGrupo,
                    NumeroEmpleadoProfesor = materia.NumeroEmpleadoProfesor,
                    Profesor = materia.Profesor,
                    Cupo = materia.Cupo,
                    Intento = evaluacion?.Intento,
                    CalificacionFinal = evaluacion?.CalificacionFinal,
                    Evaluaciones = evaluacion?.Evaluaciones ?? []
                };
            }).ToList()
        };
    }
}
