using MCP.Server.Features.ConsultarEvaluaciones.Responses;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCP.Server.Features.ConsultarEvaluaciones;

[McpServerToolType]
public sealed class ConsultarEvaluacionesTool
{
    private readonly ConsultarEvaluacionesService _service;

    public ConsultarEvaluacionesTool(ConsultarEvaluacionesService service)
    {
        _service = service;
    }

    [McpServerTool(Name = "consultar_evaluaciones")]
    [Description(
        "Consulta las evaluaciones detalladas de las materias de un estudiante para un periodo académico. Recibe estudianteId de tipo entero y periodo de tipo texto; devuelve una lista con las evaluaciones, detalles, entregables y calificación final almacenada de cada materia e intento.")]
    public async Task<IReadOnlyList<ConsultarEvaluacionesResponse>> ConsultarEvaluacionesAsync(
        [Description("ID interno del estudiante cuyas evaluaciones se desean consultar.")]
        int estudianteId,
        [Description("Periodo académico que se desea consultar, por ejemplo 2026-1.")]
        string periodo,
        CancellationToken cancellationToken = default)
    {
        return await _service.ExecuteAsync(
            estudianteId,
            periodo,
            cancellationToken);
    }
}
