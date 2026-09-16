using MongoDB.Bson.Serialization.Attributes;

namespace MCP.Server.Shared.Database.MongoDb.Documents.HistorialAcademico;

public sealed class MateriaCursadaDocument
{
    [BsonElement("grupo_materia_id")]
    public int GrupoMateriaId { get; init; }

    [BsonElement("materia_id")]
    public int MateriaId { get; init; }

    [BsonElement("grupo_id")]
    public int GrupoId { get; init; }

    [BsonElement("profesor_id")]
    public int ProfesorId { get; init; }

    [BsonElement("calificacion")]
    public double? Calificacion { get; init; }

    [BsonElement("estado")]
    public string Estado { get; init; } = string.Empty;

    [BsonElement("tipo_curso")]
    public string TipoCurso { get; init; } = string.Empty;

    [BsonElement("intento")]
    public int Intento { get; init; }

    [BsonElement("fecha_cierre")]
    public DateTime? FechaCierre { get; init; }

    [BsonElement("observaciones")]
    public string? Observaciones { get; init; }
}