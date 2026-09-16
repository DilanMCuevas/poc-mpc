using MCP.Server.Shared.Database.MongoDb.Documents.Evaluaciones;
using MongoDB.Driver;

namespace MCP.Server.Shared.Database.MongoDb.Repositories;

public sealed class EvaluacionesRepository : IEvaluacionesRepository
{
    private const string CollectionName = "evaluaciones";
    private readonly IMongoCollection<EvaluacionesDocument> _collection;

    public EvaluacionesRepository(MongoDbContext context)
    {
        _collection = context.Database
            .GetCollection<EvaluacionesDocument>(CollectionName);
    }

    public async Task<IReadOnlyList<EvaluacionesDocument>> ObtenerPorEstudianteYPeriodoAsync(
        int estudianteId,
        string periodo,
        CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(documento => documento.EstudianteId == estudianteId
                && documento.Periodo == periodo)
            .SortBy(documento => documento.MateriaId)
            .ThenBy(documento => documento.GrupoMateriaId)
            .ThenBy(documento => documento.Intento)
            .ThenBy(documento => documento.Id)
            .ToListAsync(cancellationToken);
    }
}
