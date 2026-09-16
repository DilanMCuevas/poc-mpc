using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCP.Server.Features.EliminarEstudiante;

[McpServerToolType]
public sealed class EliminarEstudianteTool
{
    private readonly EliminarEstudianteService _service;

    public EliminarEstudianteTool(EliminarEstudianteService service)
    {
        _service = service;
    }

    [McpServerTool(Name = "eliminar_estudiante", ReadOnly = false, Destructive = true)]
    [Description(
        "Elimina de MySQL al estudiante identificado por boleta únicamente si no tiene información relacionada que impida eliminarlo. Las inscripciones activas o inactivas bloquean la eliminación. No elimina registros relacionados ni documentos de MongoDB. Devuelve el resultado y, si se eliminó, los datos del estudiante afectado.")]
    public async Task<EliminarEstudianteResponse> EliminarEstudianteAsync(
        [Description("Boleta del estudiante que se desea eliminar, obligatoria y de máximo 20 caracteres.")]
        string boleta,
        CancellationToken cancellationToken = default)
    {
        return await _service.ExecuteAsync(boleta, cancellationToken);
    }
}
