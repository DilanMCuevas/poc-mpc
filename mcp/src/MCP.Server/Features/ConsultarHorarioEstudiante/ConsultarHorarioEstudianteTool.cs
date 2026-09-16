using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCP.Server.Features.ConsultarHorarioEstudiante;

[McpServerToolType]
public sealed class ConsultarHorarioEstudianteTool
{
    private readonly ConsultarHorarioEstudianteService _service;

    public ConsultarHorarioEstudianteTool(ConsultarHorarioEstudianteService service)
    {
        _service = service;
    }

    [McpServerTool(Name = "consultar_horario_estudiante")]
    [Description(
        "Consulta el horario académico de un estudiante usando los argumentos boleta y periodo de tipo texto. Devuelve los bloques de horario de sus inscripciones activas, incluyendo materia, grupo, profesor, día, horas y salón.")]
    public async Task<ConsultarHorarioEstudianteResponse?> ConsultarHorarioEstudianteAsync(
        [Description("Boleta institucional del estudiante cuyo horario se desea consultar.")]
        string boleta,
        [Description("Periodo académico para consultar las inscripciones activas del estudiante, por ejemplo 2026-1.")]
        string periodo,
        CancellationToken cancellationToken = default)
    {
        return await _service.ExecuteAsync(
            boleta,
            periodo,
            cancellationToken);
    }
}
