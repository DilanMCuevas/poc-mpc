using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCP.Server.Features.ConsultarCargaAcademica;

[McpServerToolType]
public sealed class ConsultarCargaAcademicaTool
{
    private readonly ConsultarCargaAcademicaService _service;

    public ConsultarCargaAcademicaTool(ConsultarCargaAcademicaService service)
    {
        _service = service;
    }

    [McpServerTool(Name = "consultar_carga_academica")]
    [Description(
        "Consulta la carga académica activa de un estudiante usando boleta y periodo de tipo texto. Devuelve sus materias inscritas, grupo, profesor y cupo, complementados con las evaluaciones disponibles del intento más alto de cada grupo-materia. Conserva las materias que todavía no tienen evaluaciones.")]
    public async Task<ConsultarCargaAcademicaResponse?> ConsultarCargaAcademicaAsync(
        [Description("Boleta institucional del estudiante, por ejemplo 2025000003.")]
        string boleta,
        [Description("Periodo académico de las inscripciones activas y evaluaciones, por ejemplo 2026-2.")]
        string periodo,
        CancellationToken cancellationToken = default)
    {
        return await _service.ExecuteAsync(
            boleta,
            periodo,
            cancellationToken);
    }
}
