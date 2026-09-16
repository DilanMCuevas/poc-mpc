using MCP.Server.Features.ConsultarEstudiante;

namespace MCP.Server.Features.EliminarEstudiante;

public sealed class EliminarEstudianteResponse
{
    public bool Exito { get; init; }

    public string Codigo { get; init; } = string.Empty;

    public string Mensaje { get; init; } = string.Empty;

    public ConsultarEstudianteResponse? Estudiante { get; init; }
}
