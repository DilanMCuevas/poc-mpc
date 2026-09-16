using MCP.Server.Features.ConsultarHistorial.Responses;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCP.Server.Features.ConsultarHistorial;

[McpServerToolType]
public sealed class ConsultarHistorialTool
{
    private readonly ConsultarHistorialService _service;

    public ConsultarHistorialTool(
        ConsultarHistorialService service)
    {
        _service = service;
    }

    [McpServerTool(Name = "consultar_historial")]
    [Description(
        "Consulta el historial académico de un estudiante usando su ID.")]
    public async Task<ConsultarHistorialResponse?> ConsultarHistorialAsync(
        [Description("ID interno del estudiante registrado en MySQL.")]
        int estudianteId,
        CancellationToken cancellationToken = default)
    {
        return await _service.ExecuteAsync(
            estudianteId,
            cancellationToken);
    }
}