using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MCP.Server.Features.RegistrarEvaluacion;

public sealed class RegistrarEvaluacionComponenteRequest
{
    [Description("PARCIAL, PROYECTO, PRACTICA, LABORATORIO, EXPOSICION, TAREA, EXAMEN_FINAL u OTRO.")]
    public required string Tipo { get; init; }
    public required string Nombre { get; init; }
    [Description("Calificación obtenida, entre 0 y 10.")]
    public required double Calificacion { get; init; }
    [Description("Fecha ISO 8601 con zona horaria, por ejemplo 2026-09-16T12:00:00Z.")]
    public required DateTimeOffset Fecha { get; init; }
    [Description("Porcentaje entre 0 y 100; omitir si no está disponible.")]
    public double? Ponderacion { get; init; }
    public string? Observaciones { get; init; }
    [Description("Objeto con información variable; conserva objetos, listas y campos desconocidos.")]
    public Dictionary<string, JsonElement>? Detalles { get; init; }
    public List<RegistrarEvaluacionEntregableRequest>? Entregables { get; init; }
}

public sealed class RegistrarEvaluacionEntregableRequest
{
    public required string Nombre { get; init; }
    public required string Tipo { get; init; }
    public string? Referencia { get; init; }
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? CamposAdicionales { get; init; }
}
