using System.ComponentModel;
using ModelContextProtocol.Server;

namespace MCP.Server.Features.RegistrarEvaluacion;

[McpServerToolType]
public sealed class RegistrarEvaluacionTool
{
    private readonly RegistrarEvaluacionService _service;

    public RegistrarEvaluacionTool(RegistrarEvaluacionService service)
    {
        _service = service;
    }

    [McpServerTool(Name = "registrar_evaluacion", ReadOnly = false, Destructive = false)]
    [Description("Crea un documento de evaluaciones de una materia en MongoDB. Requiere los IDs lógicos conocidos de MySQL, periodo, intento y componentes evaluados. No modifica evaluaciones existentes ni consulta MySQL. No inventar IDs ni calificaciones; calificacionFinal es opcional y no se calcula.")]
    public Task<RegistrarEvaluacionResponse> RegistrarEvaluacionAsync(
        [Description("ID positivo del estudiante en MySQL.")] int estudianteId,
        [Description("ID positivo de la asignación grupos_materia en MySQL.")] int grupoMateriaId,
        [Description("ID positivo de la materia en MySQL.")] int materiaId,
        [Description("ID positivo del grupo en MySQL.")] int grupoId,
        [Description("ID positivo del profesor en MySQL.")] int profesorId,
        [Description("Periodo académico YYYY-1 o YYYY-2.")] string periodo,
        [Description("Ocasión en que se cursa la materia, entero mayor o igual a 1.")] int intento,
        [Description("Lista no vacía de componentes con tipo, nombre, calificacion y fecha; opcionalmente ponderacion, observaciones, detalles y entregables.")]
        List<RegistrarEvaluacionComponenteRequest> evaluaciones,
        [Description("Calificación final entre 0 y 10. Omitir si aún no está disponible.")] double? calificacionFinal = null,
        CancellationToken cancellationToken = default)
    {
        return _service.ExecuteAsync(estudianteId, grupoMateriaId, materiaId, grupoId,
            profesorId, periodo, intento, evaluaciones, calificacionFinal, cancellationToken);
    }
}
