using System.Text.Json;
using System.Text.RegularExpressions;
using MCP.Server.Features.ConsultarEvaluaciones.Mappers;
using MCP.Server.Shared.Database.MongoDb.Documents.Evaluaciones;
using MCP.Server.Shared.Database.MongoDb.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MCP.Server.Features.RegistrarEvaluacion;

public sealed class RegistrarEvaluacionService
{
    private static readonly HashSet<string> Tipos =
        ["PARCIAL", "PROYECTO", "PRACTICA", "LABORATORIO", "EXPOSICION", "TAREA", "EXAMEN_FINAL", "OTRO"];
    private readonly IEvaluacionesRepository _repository;
    private readonly ILogger<RegistrarEvaluacionService> _logger;

    public RegistrarEvaluacionService(IEvaluacionesRepository repository,
        ILogger<RegistrarEvaluacionService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<RegistrarEvaluacionResponse> ExecuteAsync(
        int estudianteId, int grupoMateriaId, int materiaId, int grupoId, int profesorId,
        string periodo, int intento, List<RegistrarEvaluacionComponenteRequest> evaluaciones,
        double? calificacionFinal = null, CancellationToken cancellationToken = default)
    {
        periodo = periodo?.Trim() ?? string.Empty;
        if (estudianteId < 1 || grupoMateriaId < 1 || materiaId < 1 || grupoId < 1 || profesorId < 1 || intento < 1)
            return Error("DATOS_INVALIDOS", "Los IDs y el intento deben ser enteros positivos.");
        if (!Regex.IsMatch(periodo, @"\A[0-9]{4}-[12]\z"))
            return Error("DATOS_INVALIDOS", "El periodo debe tener formato YYYY-1 o YYYY-2.");
        if (calificacionFinal.HasValue && !EnRango(calificacionFinal.Value, 10))
            return Error("DATOS_INVALIDOS", "La calificación final debe estar entre 0 y 10.");
        if (evaluaciones is null || evaluaciones.Count == 0)
            return Error("DATOS_INVALIDOS", "Debe proporcionar al menos un componente de evaluación.");

        try
        {
            var componentes = new List<EvaluacionDocument>();
            foreach (var item in evaluaciones)
            {
                if (item is null || !TextoValido(item.Tipo) || !Tipos.Contains(item.Tipo.Trim())
                    || !TextoValido(item.Nombre) || !EnRango(item.Calificacion, 10)
                    || item.Fecha == default
                    || (item.Ponderacion.HasValue && !EnRango(item.Ponderacion.Value, 100)))
                    return Error("DATOS_INVALIDOS", "Cada evaluación requiere tipo permitido, nombre, calificación de 0 a 10, fecha válida y ponderación opcional de 0 a 100.");

                BsonArray? entregables = null;
                if (item.Entregables is not null)
                {
                    entregables = new BsonArray();
                    foreach (var entregable in item.Entregables)
                    {
                        if (entregable is null || !TextoValido(entregable.Nombre) || !TextoValido(entregable.Tipo))
                            return Error("DATOS_INVALIDOS", "Cada entregable requiere nombre y tipo no vacíos.");
                        var bson = new BsonDocument
                        {
                            { "nombre", entregable.Nombre.Trim() },
                            { "tipo", entregable.Tipo.Trim() }
                        };
                        if (entregable.Referencia is not null)
                            bson.Add("referencia", entregable.Referencia.Trim());
                        if (entregable.CamposAdicionales is not null)
                            foreach (var campo in entregable.CamposAdicionales)
                            {
                                if (campo.Key is "nombre" or "tipo" or "referencia")
                                    return Error("DATOS_INVALIDOS", "Los campos adicionales no pueden reemplazar los campos del entregable.");
                                bson.Add(campo.Key, ToBson(campo.Value));
                            }
                        entregables.Add(bson);
                    }
                }
                componentes.Add(new EvaluacionDocument
                {
                    Tipo = item.Tipo.Trim(), Nombre = item.Nombre.Trim(),
                    Calificacion = item.Calificacion, Ponderacion = item.Ponderacion,
                    Fecha = item.Fecha.UtcDateTime,
                    Detalles = item.Detalles is null ? null : new BsonDocument(
                        item.Detalles.Select(campo => new BsonElement(campo.Key, ToBson(campo.Value)))),
                    Entregables = entregables,
                    CamposAdicionales = item.Observaciones is null ? null
                        : new BsonDocument("observaciones", item.Observaciones.Trim())
                });
            }

            var ahora = DateTime.UtcNow;
            var documento = new EvaluacionesDocument
            {
                EstudianteId = estudianteId, GrupoMateriaId = grupoMateriaId, MateriaId = materiaId,
                GrupoId = grupoId, ProfesorId = profesorId, Periodo = periodo, Intento = intento,
                Evaluaciones = componentes, CalificacionFinal = calificacionFinal,
                FechaCreacion = ahora, FechaActualizacion = ahora
            };
            var respuesta = EvaluacionesMapper.ToResponse(documento);
            await _repository.InsertarAsync(documento, cancellationToken);
            return new RegistrarEvaluacionResponse
            {
                Exito = true, Codigo = "REGISTRADA", Mensaje = "Evaluación registrada correctamente.",
                Evaluacion = respuesta
            };
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException
            or OverflowException or BsonException)
        {
            return Error("DATOS_INVALIDOS", "Los componentes contienen valores que no pueden representarse como un documento BSON válido.");
        }
        catch (MongoWriteException exception) when (exception.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            return Error("DUPLICADO", "Ya existe una evaluación del estudiante para esa materia, periodo e intento.");
        }
        catch (MongoWriteException exception) when (exception.WriteError.Code == 121)
        {
            _logger.LogWarning("MongoDB rechazó el documento de evaluaciones: {ValidationDetails}",
                exception.WriteError.Details?.ToJson());
            return Error("DATOS_INVALIDOS", "El documento no cumple las restricciones de la colección evaluaciones.");
        }
        catch (MongoException exception)
        {
            _logger.LogError(exception, "Error al registrar evaluación del estudiante {EstudianteId}", estudianteId);
            return Error("ERROR", "No se pudo confirmar el registro. Consulte las evaluaciones antes de reintentar.");
        }
    }

    private static bool TextoValido(string? value) => !string.IsNullOrWhiteSpace(value);
    private static bool EnRango(double value, double max) => double.IsFinite(value) && value >= 0 && value <= max;
    private static RegistrarEvaluacionResponse Error(string codigo, string mensaje) => new()
    {
        Exito = false, Codigo = codigo, Mensaje = mensaje
    };

    // Convierte JSON estructurado sin interpretar claves de detalles como operadores Extended JSON.
    private static BsonValue ToBson(JsonElement value) => value.ValueKind switch
    {
        JsonValueKind.Object => new BsonDocument(value.EnumerateObject()
            .Select(property => new BsonElement(property.Name, ToBson(property.Value)))),
        JsonValueKind.Array => new BsonArray(value.EnumerateArray().Select(ToBson)),
        JsonValueKind.String => new BsonString(value.GetString()!),
        JsonValueKind.True => BsonBoolean.True,
        JsonValueKind.False => BsonBoolean.False,
        JsonValueKind.Number when value.TryGetInt32(out var number) => new BsonInt32(number),
        JsonValueKind.Number when value.TryGetInt64(out var number) => new BsonInt64(number),
        JsonValueKind.Number when value.TryGetDecimal(out var number) => new BsonDecimal128(number),
        JsonValueKind.Number when double.IsFinite(value.GetDouble()) => new BsonDouble(value.GetDouble()),
        JsonValueKind.Null => BsonNull.Value,
        _ => throw new ArgumentException("Valor JSON no representable.")
    };
}
