using MCP.Server.Shared.Database.MongoDb.Documents.Evaluaciones;

namespace MCP.Server.Shared.Database.MongoDb.Repositories;

public interface IEvaluacionesRepository
{
    Task<IReadOnlyList<EvaluacionesDocument>> ObtenerPorEstudianteYPeriodoAsync(
        int estudianteId,
        string periodo,
        CancellationToken cancellationToken = default);
}
