using System.Text.Json;
using System.Text.Json.Serialization;

namespace MCP.Server.Features.ConsultarEvaluaciones.Responses;

public sealed class ConsultarEvaluacionesResponse
{
    public int EstudianteId { get; init; }

    public int GrupoMateriaId { get; init; }

    public int MateriaId { get; init; }

    public int GrupoId { get; init; }

    public int ProfesorId { get; init; }

    public string Periodo { get; init; } = string.Empty;

    public int Intento { get; init; }

    public IReadOnlyList<EvaluacionResponse> Evaluaciones { get; init; } = [];

    public double? CalificacionFinal { get; init; }

    public DateTime FechaCreacion { get; init; }

    public DateTime FechaActualizacion { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? CamposAdicionales { get; init; }
}
