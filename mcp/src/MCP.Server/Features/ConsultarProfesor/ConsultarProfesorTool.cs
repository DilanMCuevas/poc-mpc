using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCP.Server.Features.ConsultarProfesor;

[McpServerToolType]
public sealed class ConsultarProfesorTool
{
    private readonly ConsultarProfesorService _service;

    public ConsultarProfesorTool(ConsultarProfesorService service)
    {
        _service = service;
    }

    [McpServerTool(Name = "consultar_profesor")]
    [Description(
        "Consulta la información general de un profesor usando el argumento numeroEmpleado de tipo texto, por ejemplo EMP001.")]
    public async Task<ConsultarProfesorResponse?> ConsultarProfesorAsync(
        [Description("Número de empleado del profesor, por ejemplo EMP001.")]
        string numeroEmpleado,
        CancellationToken cancellationToken = default)
    {
        return await _service.ExecuteAsync(
            numeroEmpleado,
            cancellationToken);
    }
}
