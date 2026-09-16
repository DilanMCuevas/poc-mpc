using MCP.Server.Features.ConsultarHistorial.Mappers;
using MCP.Server.Features.ConsultarHistorial.Responses;
using MCP.Server.Shared.Database.MongoDb.Repositories;

namespace MCP.Server.Features.ConsultarHistorial;

public sealed class ConsultarHistorialService
{
    private readonly IHistorialAcademicoRepository _repository;

    public ConsultarHistorialService(
        IHistorialAcademicoRepository repository)
    {
        _repository = repository;
    }

    public async Task<ConsultarHistorialResponse?> ExecuteAsync(
        int estudianteId,
        CancellationToken cancellationToken = default)
    {
        var document =
            await _repository.ObtenerPorEstudianteAsync(
                estudianteId,
                cancellationToken);

        if (document is null)
        {
            return null;
        }

        return HistorialAcademicoMapper.ToResponse(document);
    }
}