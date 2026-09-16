using MCP.Server.Features.ConsultarEvaluaciones.Responses;

namespace MCP.Server.Features.ConsultarCargaAcademica;

public sealed class ConsultarCargaAcademicaResponse
{
    public int Id { get; init; }

    public string Boleta { get; init; } = string.Empty;

    public string Nombre { get; init; } = string.Empty;

    public string Periodo { get; init; } = string.Empty;

    public IReadOnlyList<MateriaCargaAcademicaResponse> Materias { get; init; } = [];
}

public sealed class MateriaCargaAcademicaResponse
{
    public int GrupoMateriaId { get; init; }

    public int MateriaId { get; init; }

    public string ClaveMateria { get; init; } = string.Empty;

    public string Materia { get; init; } = string.Empty;

    public int Creditos { get; init; }

    public string ClaveGrupo { get; init; } = string.Empty;

    public string NumeroEmpleadoProfesor { get; init; } = string.Empty;

    public string Profesor { get; init; } = string.Empty;

    public int Cupo { get; init; }

    public int? Intento { get; init; }

    public double? CalificacionFinal { get; init; }

    public IReadOnlyList<EvaluacionResponse> Evaluaciones { get; init; } = [];
}
