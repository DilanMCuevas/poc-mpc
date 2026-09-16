using MCP.Server.Features.ConsultarEstudiante;

namespace MCP.Server.Features.RegistrarEstudiante;

public sealed class RegistrarEstudianteResponse
{
    public bool Exito { get; init; }

    public string Mensaje { get; init; } = string.Empty;

    public ConsultarEstudianteResponse? Estudiante { get; init; }
}
