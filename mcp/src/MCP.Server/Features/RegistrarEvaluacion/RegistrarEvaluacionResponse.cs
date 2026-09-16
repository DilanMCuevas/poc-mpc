using MCP.Server.Features.ConsultarEvaluaciones.Responses;

namespace MCP.Server.Features.RegistrarEvaluacion;

public sealed class RegistrarEvaluacionResponse
{
    public bool Exito { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Mensaje { get; init; } = string.Empty;
    public ConsultarEvaluacionesResponse? Evaluacion { get; init; }
}
