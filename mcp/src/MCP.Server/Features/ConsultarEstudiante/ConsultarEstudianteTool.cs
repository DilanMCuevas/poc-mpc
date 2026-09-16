using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCP.Server.Features.ConsultarEstudiante;

[McpServerToolType]
public sealed class ConsultarEstudianteTool
{
    private readonly ConsultarEstudianteService _service;

    public ConsultarEstudianteTool(ConsultarEstudianteService service)
    {
        _service = service;
    }
    [McpServerTool]
    [Description(
    "Consulta la información general de un estudiante usando su boleta.")]
    public async Task<ConsultarEstudianteResponse?> ConsultarEstudianteAsync(
        [Description("Boleta institucional del estudiante.")]
        string boleta,
        CancellationToken cancellationToken = default)
    {
        return await _service.ExecuteAsync(
            boleta,
            cancellationToken);
    }
}