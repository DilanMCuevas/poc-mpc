namespace MCP.Server.Features.ConsultarHistorial.Responses;

public sealed class MateriaHistorialResponse
{
    public int GrupoMateriaId { get; init; }

    public int MateriaId { get; init; }

    public int GrupoId { get; init; }

    public int ProfesorId { get; init; }

    public double? Calificacion { get; init; }

    public string Estado { get; init; } = string.Empty;

    public string TipoCurso { get; init; } = string.Empty;

    public int Intento { get; init; }

    public DateTime? FechaCierre { get; init; }

    public string? Observaciones { get; init; }
}