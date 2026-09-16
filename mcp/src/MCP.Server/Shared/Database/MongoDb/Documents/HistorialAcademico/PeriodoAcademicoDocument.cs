using MongoDB.Bson.Serialization.Attributes;

namespace MCP.Server.Shared.Database.MongoDb.Documents.HistorialAcademico;

public sealed class PeriodoAcademicoDocument
{
    [BsonElement("periodo")]
    public string Periodo { get; init; } = string.Empty;

    [BsonElement("semestre")]
    public int Semestre { get; init; }

    [BsonElement("materias")]
    public List<MateriaCursadaDocument> Materias { get; init; } = [];
}