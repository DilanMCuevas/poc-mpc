namespace MCP.Server.Features.ConsultarGrupo;

public sealed class ConsultarGrupoResponse
{
    public int Id { get; init; }

    public string ClaveGrupo { get; init; } = string.Empty;

    public string Periodo { get; init; } = string.Empty;

    public int Semestre { get; init; }

    public IReadOnlyList<MateriaGrupoResponse> Materias { get; init; } = [];
}

public sealed class MateriaGrupoResponse
{
    public int GrupoMateriaId { get; init; }

    public string Clave { get; init; } = string.Empty;

    public string Nombre { get; init; } = string.Empty;

    public int Creditos { get; init; }

    public string NumeroEmpleadoProfesor { get; init; } = string.Empty;

    public string Profesor { get; init; } = string.Empty;

    public int Cupo { get; init; }
}
