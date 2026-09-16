using MCP.Server.Features.ConsultarEstudiante;
using MCP.Server.Features.ConsultarHistorial.Responses;

namespace MCP.Server.Features.ConsultarExpediente;

public sealed class ConsultarExpedienteResponse
{
    public required ConsultarEstudianteResponse Estudiante { get; init; }

    public ConsultarHistorialResponse? Historial { get; init; }
}