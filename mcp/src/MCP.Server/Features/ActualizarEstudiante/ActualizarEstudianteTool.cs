using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCP.Server.Features.ActualizarEstudiante;

[McpServerToolType]
public sealed class ActualizarEstudianteTool
{
    private readonly ActualizarEstudianteService _service;

    public ActualizarEstudianteTool(ActualizarEstudianteService service)
    {
        _service = service;
    }

    [McpServerTool(Name = "actualizar_estudiante", ReadOnly = false, Destructive = true)]
    [Description(
        "Actualiza parcialmente un estudiante existente identificado por boleta, sin cambiar su boleta. Proporciona al menos uno de nombre, correo, telefono o programaAcademico. Los campos omitidos o null conservan su valor; los textos vacíos se rechazan. Devuelve el resultado y el estudiante completo actualizado.")]
    public async Task<ActualizarEstudianteResponse> ActualizarEstudianteAsync(
        [Description("Boleta del estudiante que se desea actualizar, máximo 20 caracteres. No se modifica.")]
        string boleta,
        [Description("Nuevo nombre completo, máximo 150 caracteres. Omitir para conservar el actual.")]
        string? nombre = null,
        [Description("Nuevo correo electrónico, máximo 100 caracteres. Omitir para conservar el actual.")]
        string? correo = null,
        [Description("Nuevo teléfono único, máximo 20 caracteres. Omitir para conservar el actual.")]
        string? telefono = null,
        [Description("Clave o nombre exacto del nuevo programa académico, por ejemplo IIA. Omitir para conservar el actual.")]
        string? programaAcademico = null,
        CancellationToken cancellationToken = default)
    {
        return await _service.ExecuteAsync(
            boleta,
            nombre,
            correo,
            telefono,
            programaAcademico,
            cancellationToken);
    }
}
