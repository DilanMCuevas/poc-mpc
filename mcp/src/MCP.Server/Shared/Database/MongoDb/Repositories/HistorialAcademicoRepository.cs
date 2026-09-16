using MCP.Server.Shared.Database.MongoDb.Documents.HistorialAcademico;
using MongoDB.Driver;

namespace MCP.Server.Shared.Database.MongoDb.Repositories;

public sealed class HistorialAcademicoRepository
    : IHistorialAcademicoRepository
{
    private const string CollectionName = "historial_academico";

    private readonly IMongoCollection<HistorialAcademicoDocument> _collection;

    public HistorialAcademicoRepository(MongoDbContext context)
    {
        _collection = context.Database
            .GetCollection<HistorialAcademicoDocument>(CollectionName);
    }

    public async Task<HistorialAcademicoDocument?>
        ObtenerPorEstudianteAsync(
            int estudianteId,
            CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(documento =>
                documento.EstudianteId == estudianteId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}