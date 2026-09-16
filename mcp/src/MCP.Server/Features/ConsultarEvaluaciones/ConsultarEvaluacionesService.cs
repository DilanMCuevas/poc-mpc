using MCP.Server.Features.ConsultarEvaluaciones.Mappers;
using MCP.Server.Features.ConsultarEvaluaciones.Responses;
using MCP.Server.Shared.Database.MongoDb.Repositories;

namespace MCP.Server.Features.ConsultarEvaluaciones;

public sealed class ConsultarEvaluacionesService
{
    private readonly IEvaluacionesRepository _repository;

    public ConsultarEvaluacionesService(IEvaluacionesRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ConsultarEvaluacionesResponse>> ExecuteAsync(
        int estudianteId,
        string periodo,
        CancellationToken cancellationToken = default)
    {
        var documents = await _repository.ObtenerPorEstudianteYPeriodoAsync(
            estudianteId,
            periodo,
            cancellationToken);

        return documents.Select(EvaluacionesMapper.ToResponse).ToList();
    }
}
