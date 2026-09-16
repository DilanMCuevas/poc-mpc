namespace MCP.Server.Features.ConsultarHistorial.Responses;

public sealed class PeriodoHistorialResponse
{
    public string Periodo { get; init; } = string.Empty;

    public int Semestre { get; init; }

    public IReadOnlyList<MateriaHistorialResponse> Materias { get; init; }
        = [];
}