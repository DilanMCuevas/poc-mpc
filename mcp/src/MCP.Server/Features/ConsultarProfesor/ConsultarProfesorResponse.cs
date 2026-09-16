namespace MCP.Server.Features.ConsultarProfesor;

public sealed class ConsultarProfesorResponse
{
    public int Id { get; init; }

    public string NumeroEmpleado { get; init; } = string.Empty;

    public string Nombre { get; init; } = string.Empty;

    public string Correo { get; init; } = string.Empty;

    public string Telefono { get; init; } = string.Empty;
}
