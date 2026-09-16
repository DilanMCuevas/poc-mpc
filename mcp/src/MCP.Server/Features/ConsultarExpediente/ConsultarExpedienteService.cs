using MCP.Server.Features.ConsultarEstudiante;
using MCP.Server.Features.ConsultarHistorial;
namespace MCP.Server.Features.ConsultarExpediente;

public sealed class ConsultarExpedienteService
{
    private readonly ConsultarEstudianteService _estudianteService;
    private readonly ConsultarHistorialService _historialService;

    public ConsultarExpedienteService(
        ConsultarEstudianteService estudianteService,
        ConsultarHistorialService historialService)
    {
        _estudianteService = estudianteService;
        _historialService = historialService;
    }

    public async Task<ConsultarExpedienteResponse?> ExecuteAsync(
        string boleta,
        CancellationToken cancellationToken = default)
    {
        var estudiante = await _estudianteService.ExecuteAsync(
            boleta,
            cancellationToken);

        if (estudiante is null)
        {
            return null;
        }

        var historial = await _historialService.ExecuteAsync(
            estudiante.Id,
            cancellationToken);

        return new ConsultarExpedienteResponse
        {
            Estudiante = estudiante,
            Historial = historial
        };
    }
}