using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MCP.Server.Shared.Database.MongoDb.Documents.Evaluaciones;

public sealed class EvaluacionesDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; } = string.Empty;

    [BsonElement("estudiante_id")]
    public int EstudianteId { get; init; }

    [BsonElement("grupo_materia_id")]
    public int GrupoMateriaId { get; init; }

    [BsonElement("materia_id")]
    public int MateriaId { get; init; }

    [BsonElement("grupo_id")]
    public int GrupoId { get; init; }

    [BsonElement("profesor_id")]
    public int ProfesorId { get; init; }

    [BsonElement("periodo")]
    public string Periodo { get; init; } = string.Empty;

    [BsonElement("intento")]
    public int Intento { get; init; }

    [BsonElement("evaluaciones")]
    public List<EvaluacionDocument> Evaluaciones { get; init; } = [];

    [BsonElement("calificacion_final")]
    public double? CalificacionFinal { get; init; }

    [BsonElement("fecha_creacion")]
    public DateTime FechaCreacion { get; init; }

    [BsonElement("fecha_actualizacion")]
    public DateTime FechaActualizacion { get; init; }

    [BsonExtraElements]
    public BsonDocument? CamposAdicionales { get; init; }
}
