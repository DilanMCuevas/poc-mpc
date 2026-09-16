namespace MCP.Server.Features.ConsultarMateria;

public sealed class ConsultarMateriaResponse
{
    public int Id { get; init; }

    public string Clave { get; init; } = string.Empty;

    public string Nombre { get; init; } = string.Empty;

    public int Creditos { get; init; }

    public int Semestre { get; init; }

    public string ProgramaAcademico { get; init; } = string.Empty;
}
