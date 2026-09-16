using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MCP.Server.Shared.Database.MongoDb.Documents.HistorialAcademico;

public sealed class HistorialAcademicoDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; } = string.Empty;

    [BsonElement("estudiante_id")]
    public int EstudianteId { get; init; }

    [BsonElement("periodos")]
    public List<PeriodoAcademicoDocument> Periodos { get; init; } = [];

    [BsonElement("fecha_creacion")]
    public DateTime FechaCreacion { get; init; }

    [BsonElement("fecha_actualizacion")]
    public DateTime FechaActualizacion { get; init; }
}