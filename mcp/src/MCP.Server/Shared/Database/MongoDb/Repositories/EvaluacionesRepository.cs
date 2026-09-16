using MCP.Server.Shared.Database.MongoDb.Documents.Evaluaciones;
using MongoDB.Driver;
using MongoDB.Bson;

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

    public async Task InsertarAsync(EvaluacionesDocument documento, CancellationToken cancellationToken = default)
    {
        // Los modelos de lectura admiten null, pero el validador exige omitir los opcionales ausentes.
        var componentes = new BsonArray();
        foreach (var componente in documento.Evaluaciones)
        {
            var bson = componente.ToBsonDocument();
            foreach (var campo in new[] { "ponderacion", "detalles", "entregables" })
                if (bson.TryGetValue(campo, out var valor) && valor.IsBsonNull)
                    bson.Remove(campo);
            componentes.Add(bson);
        }
        var registro = new BsonDocument
        {
            { "estudiante_id", documento.EstudianteId },
            { "grupo_materia_id", documento.GrupoMateriaId },
            { "materia_id", documento.MateriaId },
            { "grupo_id", documento.GrupoId },
            { "profesor_id", documento.ProfesorId },
            { "periodo", documento.Periodo },
            { "intento", documento.Intento },
            { "evaluaciones", componentes },
            { "fecha_creacion", documento.FechaCreacion },
            { "fecha_actualizacion", documento.FechaActualizacion }
        };
        if (documento.CalificacionFinal.HasValue)
            registro.Add("calificacion_final", documento.CalificacionFinal.Value);

        // Sin _id explícito: el driver genera ObjectId. No hay upsert ni escritura sobre documentos existentes.
        await _collection.Database.GetCollection<BsonDocument>(CollectionName)
            .InsertOneAsync(registro, cancellationToken: cancellationToken);
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
