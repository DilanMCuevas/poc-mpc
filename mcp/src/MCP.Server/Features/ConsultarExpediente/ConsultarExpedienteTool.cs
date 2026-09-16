using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCP.Server.Features.ConsultarExpediente;

[McpServerToolType]
public sealed class ConsultarExpedienteTool
{
    private readonly ConsultarExpedienteService _service;

    public ConsultarExpedienteTool(
        ConsultarExpedienteService service)
    {
        _service = service;
    }

    [McpServerTool(Name = "consultar_expediente")]
    [Description(
        "Consulta el expediente completo de un estudiante usando su boleta, combinando información de MySQL y MongoDB.")]
    public async Task<ConsultarExpedienteResponse?> ConsultarExpedienteAsync(
        [Description("Boleta institucional del estudiante.")]
        string boleta,
        CancellationToken cancellationToken = default)
    {
        return await _service.ExecuteAsync(
            boleta,
            cancellationToken);
    }
}