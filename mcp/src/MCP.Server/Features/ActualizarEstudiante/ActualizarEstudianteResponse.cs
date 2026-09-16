using MCP.Server.Features.ConsultarEstudiante;

namespace MCP.Server.Features.ActualizarEstudiante;

public sealed class ActualizarEstudianteResponse
{
    public bool Exito { get; init; }

    public string Codigo { get; init; } = string.Empty;

    public string Mensaje { get; init; } = string.Empty;

    public ConsultarEstudianteResponse? Estudiante { get; init; }
}
