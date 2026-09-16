using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCP.Server.Features.ConsultarMateria;

[McpServerToolType]
public sealed class ConsultarMateriaTool
{
    private readonly ConsultarMateriaService _service;

    public ConsultarMateriaTool(ConsultarMateriaService service)
    {
        _service = service;
    }

    [McpServerTool(Name = "consultar_materia")]
    [Description(
        "Consulta la información general de una materia usando el argumento clave de tipo texto, por ejemplo ISC202.")]
    public async Task<ConsultarMateriaResponse?> ConsultarMateriaAsync(
        [Description("Clave de la materia, por ejemplo ISC202.")]
        string clave,
        CancellationToken cancellationToken = default)
    {
        return await _service.ExecuteAsync(
            clave,
            cancellationToken);
    }
}
