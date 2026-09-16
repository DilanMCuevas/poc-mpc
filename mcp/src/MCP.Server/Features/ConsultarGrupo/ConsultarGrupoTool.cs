using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCP.Server.Features.ConsultarGrupo;

[McpServerToolType]
public sealed class ConsultarGrupoTool
{
    private readonly ConsultarGrupoService _service;

    public ConsultarGrupoTool(ConsultarGrupoService service)
    {
        _service = service;
    }

    [McpServerTool(Name = "consultar_grupo")]
    [Description(
        "Consulta un grupo académico mediante los argumentos claveGrupo y periodo de tipo texto. Devuelve sus datos básicos, materias, profesor asignado y cupo de cada grupo-materia.")]
    public async Task<ConsultarGrupoResponse?> ConsultarGrupoAsync(
        [Description("Clave del grupo académico que se desea consultar.")]
        string claveGrupo,
        [Description("Periodo académico del grupo, por ejemplo 2026-1. Junto con claveGrupo identifica al grupo.")]
        string periodo,
        CancellationToken cancellationToken = default)
    {
        return await _service.ExecuteAsync(
            claveGrupo,
            periodo,
            cancellationToken);
    }
}
