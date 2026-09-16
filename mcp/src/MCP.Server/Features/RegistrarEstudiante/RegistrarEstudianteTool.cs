using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCP.Server.Features.RegistrarEstudiante;

[McpServerToolType]
public sealed class RegistrarEstudianteTool
{
    private readonly RegistrarEstudianteService _service;

    public RegistrarEstudianteTool(RegistrarEstudianteService service)
    {
        _service = service;
    }

    [McpServerTool(Name = "registrar_estudiante", ReadOnly = false, Destructive = false)]
    [Description(
        "Registra un estudiante nuevo. Requiere boleta, nombre, correo, telefono y programaAcademico como texto. programaAcademico debe ser la clave o el nombre exacto de un programa existente. No crea duplicados de boleta ni teléfono; devuelve el resultado y los datos del estudiante creado.")]
    public async Task<RegistrarEstudianteResponse> RegistrarEstudianteAsync(
        [Description("Boleta institucional única del estudiante, máximo 20 caracteres.")]
        string boleta,
        [Description("Nombre completo del estudiante, máximo 150 caracteres.")]
        string nombre,
        [Description("Correo electrónico del estudiante, máximo 100 caracteres.")]
        string correo,
        [Description("Teléfono único del estudiante, máximo 20 caracteres.")]
        string telefono,
        [Description("Clave o nombre exacto de un programa académico existente, por ejemplo ISC o Ingeniería en Sistemas Computacionales.")]
        string programaAcademico,
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
