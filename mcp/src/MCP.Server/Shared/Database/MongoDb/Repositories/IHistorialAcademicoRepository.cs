using MCP.Server.Shared.Database.MongoDb.Documents.HistorialAcademico;

namespace MCP.Server.Shared.Database.MongoDb.Repositories;

public interface IHistorialAcademicoRepository
{
    Task<HistorialAcademicoDocument?> ObtenerPorEstudianteAsync(
        int estudianteId,
        CancellationToken cancellationToken = default);
}