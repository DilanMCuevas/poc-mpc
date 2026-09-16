using System.Text.Json;
using MCP.Server.Features.ConsultarEvaluaciones.Responses;
using MCP.Server.Shared.Database.MongoDb.Documents.Evaluaciones;
using MongoDB.Bson;
using MongoDB.Bson.IO;

namespace MCP.Server.Features.ConsultarEvaluaciones.Mappers;

public static class EvaluacionesMapper
{
    private static readonly JsonWriterSettings JsonSettings = new()
    {
        OutputMode = JsonOutputMode.RelaxedExtendedJson
    };

    public static ConsultarEvaluacionesResponse ToResponse(EvaluacionesDocument document)
    {
        return new ConsultarEvaluacionesResponse
        {
            EstudianteId = document.EstudianteId,
            GrupoMateriaId = document.GrupoMateriaId,
            MateriaId = document.MateriaId,
            GrupoId = document.GrupoId,
            ProfesorId = document.ProfesorId,
            Periodo = document.Periodo,
            Intento = document.Intento,
            Evaluaciones = document.Evaluaciones.Select(ToResponse).ToList(),
            CalificacionFinal = document.CalificacionFinal,
            FechaCreacion = document.FechaCreacion,
            FechaActualizacion = document.FechaActualizacion,
            CamposAdicionales = ToAdditionalFields(document.CamposAdicionales)
        };
    }

    private static EvaluacionResponse ToResponse(EvaluacionDocument document)
    {
        return new EvaluacionResponse
        {
            Tipo = document.Tipo,
            Nombre = document.Nombre,
            Calificacion = document.Calificacion,
            Ponderacion = document.Ponderacion,
            Fecha = document.Fecha,
            Detalles = ToJson(document.Detalles),
            Entregables = ToJson(document.Entregables),
            CamposAdicionales = ToAdditionalFields(document.CamposAdicionales)
        };
    }

    private static JsonElement? ToJson(BsonValue? value)
    {
        if (value is null)
        {
            return null;
        }

        using var json = JsonDocument.Parse(value.ToJson(JsonSettings));
        return json.RootElement.Clone();
    }

    private static Dictionary<string, JsonElement>? ToAdditionalFields(BsonDocument? document)
    {
        return document?.Elements.ToDictionary(
            element => element.Name,
            element => ToJson(element.Value)!.Value);
    }
}
