namespace MCP.Server.Features.ConsultarHistorial.Responses;

public sealed class ConsultarHistorialResponse
{
    public int EstudianteId { get; init; }

    public IReadOnlyList<PeriodoHistorialResponse> Periodos { get; init; }
        = [];
}