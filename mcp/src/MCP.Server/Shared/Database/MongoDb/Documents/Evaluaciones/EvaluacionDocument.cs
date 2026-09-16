using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MCP.Server.Shared.Database.MongoDb.Documents.Evaluaciones;

public sealed class EvaluacionDocument
{
    [BsonElement("tipo")]
    public string Tipo { get; init; } = string.Empty;

    [BsonElement("nombre")]
    public string Nombre { get; init; } = string.Empty;

    [BsonElement("calificacion")]
    public double? Calificacion { get; init; }

    [BsonElement("ponderacion")]
    public double? Ponderacion { get; init; }

    [BsonElement("fecha")]
    public DateTime Fecha { get; init; }

    [BsonElement("detalles")]
    public BsonDocument? Detalles { get; init; }

    [BsonElement("entregables")]
    public BsonArray? Entregables { get; init; }

    [BsonExtraElements]
    public BsonDocument? CamposAdicionales { get; init; }
}
