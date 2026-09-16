namespace MCP.Server.Features.ConsultarEstudiante;

public sealed class ConsultarEstudianteResponse
{
    public int Id { get; init; }

    public string Boleta { get; init; } = string.Empty;

    public string Nombre { get; init; } = string.Empty;

    public string Correo { get; init; } = string.Empty;

    public string Telefono { get; init; } = string.Empty;

    public string ProgramaAcademico { get; init; } = string.Empty;
}