namespace MCP.Server.Features.ConsultarHorarioEstudiante;

public sealed class ConsultarHorarioEstudianteResponse
{
    public int Id { get; init; }

    public string Boleta { get; init; } = string.Empty;

    public string Nombre { get; init; } = string.Empty;

    public string Periodo { get; init; } = string.Empty;

    public IReadOnlyList<BloqueHorarioResponse> Horario { get; init; } = [];
}

public sealed class BloqueHorarioResponse
{
    public int GrupoMateriaId { get; init; }

    public string ClaveMateria { get; init; } = string.Empty;

    public string Materia { get; init; } = string.Empty;

    public string ClaveGrupo { get; init; } = string.Empty;

    public string NumeroEmpleadoProfesor { get; init; } = string.Empty;

    public string Profesor { get; init; } = string.Empty;

    public string Dia { get; init; } = string.Empty;

    public TimeSpan HoraInicio { get; init; }

    public TimeSpan HoraFin { get; init; }

    public string Salon { get; init; } = string.Empty;

    public string TipoSalon { get; init; } = string.Empty;
}
