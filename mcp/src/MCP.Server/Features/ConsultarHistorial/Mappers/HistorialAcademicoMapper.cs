using MCP.Server.Features.ConsultarHistorial.Responses;
using MCP.Server.Shared.Database.MongoDb.Documents.HistorialAcademico;

namespace MCP.Server.Features.ConsultarHistorial.Mappers;

public static class HistorialAcademicoMapper
{
    public static ConsultarHistorialResponse ToResponse(
        HistorialAcademicoDocument document)
    {
        return new ConsultarHistorialResponse
        {
            EstudianteId = document.EstudianteId,

            Periodos = document.Periodos
                .Select(periodo => new PeriodoHistorialResponse
                {
                    Periodo = periodo.Periodo,
                    Semestre = periodo.Semestre,

                    Materias = periodo.Materias
                        .Select(materia => new MateriaHistorialResponse
                        {
                            GrupoMateriaId = materia.GrupoMateriaId,
                            MateriaId = materia.MateriaId,
                            GrupoId = materia.GrupoId,
                            ProfesorId = materia.ProfesorId,
                            Calificacion = materia.Calificacion,
                            Estado = materia.Estado,
                            TipoCurso = materia.TipoCurso,
                            Intento = materia.Intento,
                            FechaCierre = materia.FechaCierre,
                            Observaciones = materia.Observaciones
                        })
                        .ToList()
                })
                .ToList()
        };
    }
}