using MCP.Server.Shared.Database.MongoDb.Documents.Evaluaciones;

namespace MCP.Server.Shared.Database.MongoDb.Repositories;

public interface IEvaluacionesRepository
{
    Task InsertarAsync(EvaluacionesDocument documento, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EvaluacionesDocument>> ObtenerPorEstudianteYPeriodoAsync(
        int estudianteId,
        string periodo,
        CancellationToken cancellationToken = default);
}
