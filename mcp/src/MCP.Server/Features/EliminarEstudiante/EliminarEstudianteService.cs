using Dapper;
using MCP.Server.Features.ConsultarEstudiante;
using MCP.Server.Shared.Database.MySql;
using MySqlConnector;

namespace MCP.Server.Features.EliminarEstudiante;

public sealed class EliminarEstudianteService
{
    private readonly MySqlConnectionFactory _connectionFactory;

    private const string BuscarSql = """
        SELECT id
        FROM estudiantes
        WHERE boleta = @Boleta
        FOR UPDATE;
        """;

    private const string DependenciasSql = """
        SELECT EXISTS(
            SELECT 1 FROM inscripciones
            WHERE estudiante_id = @Id);
        """;

    private const string EstudianteSql = """
        SELECT
            e.id AS Id,
            e.boleta AS Boleta,
            e.nombre AS Nombre,
            e.correo AS Correo,
            e.telefono AS Telefono,
            pa.nombre AS ProgramaAcademico
        FROM estudiantes e
        INNER JOIN programas_academicos pa
            ON pa.id = e.programa_academico_id
        WHERE e.id = @Id;
        """;

    private const string DeleteSql = """
        DELETE FROM estudiantes
        WHERE id = @Id AND boleta = @Boleta;
        """;

    public EliminarEstudianteService(MySqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<EliminarEstudianteResponse> ExecuteAsync(
        string boleta,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(boleta))
        {
            return Failure("DATOS_INVALIDOS", "El dato boleta es obligatorio.");
        }

        if (boleta.Trim().Length > 20)
        {
            return Failure("DATOS_INVALIDOS", "El dato boleta no puede exceder 20 caracteres.");
        }

        if (boleta.Any(char.IsControl))
        {
            return Failure("DATOS_INVALIDOS", "El dato boleta contiene caracteres de control no válidos.");
        }

        boleta = boleta.Trim();

        try
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync(cancellationToken);
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            var id = await connection.QuerySingleOrDefaultAsync<int?>(new CommandDefinition(
                commandText: BuscarSql,
                parameters: new { Boleta = boleta },
                transaction: transaction,
                cancellationToken: cancellationToken));

            if (id is null)
            {
                return Failure("ESTUDIANTE_NO_ENCONTRADO", "No existe un estudiante con esa boleta.");
            }

            var tieneDependencias = await connection.QuerySingleAsync<bool>(new CommandDefinition(
                commandText: DependenciasSql,
                parameters: new { Id = id.Value },
                transaction: transaction,
                cancellationToken: cancellationToken));

            if (tieneDependencias)
            {
                return Dependencias();
            }

            var estudiante = await connection.QuerySingleAsync<ConsultarEstudianteResponse>(
                new CommandDefinition(
                    commandText: EstudianteSql,
                    parameters: new { Id = id.Value },
                    transaction: transaction,
                    cancellationToken: cancellationToken));

            var filasAfectadas = await connection.ExecuteAsync(new CommandDefinition(
                commandText: DeleteSql,
                parameters: new { Id = id.Value, Boleta = boleta },
                transaction: transaction,
                cancellationToken: cancellationToken));

            if (filasAfectadas != 1)
            {
                return Failure("ERROR_ELIMINACION",
                    "No se confirmó la eliminación de exactamente un estudiante. La operación se revierte.");
            }

            await transaction.CommitAsync(cancellationToken);

            return new EliminarEstudianteResponse
            {
                Exito = true,
                Codigo = "ELIMINADO",
                Mensaje = "Estudiante eliminado correctamente de MySQL.",
                Estudiante = estudiante
            };
        }
        catch (MySqlException exception) when (
            exception.ErrorCode is MySqlErrorCode.RowIsReferenced or MySqlErrorCode.RowIsReferenced2)
        {
            // Foreign keys remain the final safeguard, including concurrent references.
            return Dependencias();
        }
        catch (MySqlException)
        {
            // Do not expose SQL/connection details or claim success on an unconfirmed commit.
            return Failure("ERROR_ELIMINACION",
                "No fue posible confirmar la eliminación. Consulta el estado del estudiante antes de reintentar.");
        }
        // await using rolls back uncommitted transactions, including cancellation paths.
    }

    private static EliminarEstudianteResponse Dependencias() => Failure(
        "ESTUDIANTE_CON_DEPENDENCIAS",
        "El estudiante posee información relacionada y no puede eliminarse mediante esta operación.");

    private static EliminarEstudianteResponse Failure(string codigo, string mensaje) => new()
    {
        Exito = false,
        Codigo = codigo,
        Mensaje = mensaje
    };
}
