using System.Text.Json;
using System.Text.Json.Serialization;

namespace MCP.Server.Features.ConsultarEvaluaciones.Responses;

public sealed class EvaluacionResponse
{
    public string Tipo { get; init; } = string.Empty;

    public string Nombre { get; init; } = string.Empty;

    public double? Calificacion { get; init; }

    public double? Ponderacion { get; init; }

    public DateTime Fecha { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonElement? Detalles { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonElement? Entregables { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? CamposAdicionales { get; init; }
}
