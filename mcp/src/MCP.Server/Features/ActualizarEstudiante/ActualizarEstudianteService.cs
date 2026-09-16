using System.Net.Mail;
using Dapper;
using MCP.Server.Features.ConsultarEstudiante;
using MCP.Server.Shared.Database.MySql;
using MySqlConnector;

namespace MCP.Server.Features.ActualizarEstudiante;

public sealed class ActualizarEstudianteService
{
    private readonly MySqlConnectionFactory _connectionFactory;

    private const string BuscarSql = """
        SELECT id
        FROM estudiantes
        WHERE boleta = @Boleta
        FOR UPDATE;
        """;

    private const string ProgramaSql = """
        SELECT id
        FROM programas_academicos
        WHERE clave = @ProgramaAcademico OR nombre = @ProgramaAcademico
        LIMIT 2
        FOR SHARE;
        """;

    private const string TelefonoSql = """
        SELECT EXISTS(
            SELECT 1 FROM estudiantes
            WHERE telefono = @Telefono AND id <> @Id);
        """;

    private const string UpdateSql = """
        UPDATE estudiantes
        SET nombre = COALESCE(@Nombre, nombre),
            correo = COALESCE(@Correo, correo),
            telefono = COALESCE(@Telefono, telefono),
            programa_academico_id = COALESCE(@ProgramaAcademicoId, programa_academico_id)
        WHERE id = @Id;
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

    public ActualizarEstudianteService(MySqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ActualizarEstudianteResponse> ExecuteAsync(
        string boleta,
        string? nombre = null,
        string? correo = null,
        string? telefono = null,
        string? programaAcademico = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(boleta))
        {
            return Failure("DATOS_INVALIDOS", "El dato boleta es obligatorio.");
        }

        var inputs = new (string Name, string? Value, int MaxLength)[]
        {
            (nameof(boleta), boleta, 20),
            (nameof(nombre), nombre, 150),
            (nameof(correo), correo, 100),
            (nameof(telefono), telefono, 20),
            (nameof(programaAcademico), programaAcademico, 150)
        };

        foreach (var input in inputs)
        {
            if (input.Value is null)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(input.Value))
            {
                return Failure("DATOS_INVALIDOS", $"El dato {input.Name} no puede estar vacío.");
            }

            if (input.Value.Trim().Length > input.MaxLength)
            {
                return Failure("DATOS_INVALIDOS",
                    $"El dato {input.Name} no puede exceder {input.MaxLength} caracteres.");
            }

            if (input.Value.Any(char.IsControl))
            {
                return Failure("DATOS_INVALIDOS",
                    $"El dato {input.Name} contiene caracteres de control no válidos.");
            }
        }

        boleta = boleta.Trim();
        nombre = nombre?.Trim();
        correo = correo?.Trim();
        telefono = telefono?.Trim();
        programaAcademico = programaAcademico?.Trim();

        if (nombre is null && correo is null && telefono is null && programaAcademico is null)
        {
            return Failure("SIN_CAMPOS", "Proporciona al menos un campo para actualizar además de la boleta.");
        }

        if (correo is not null && (!MailAddress.TryCreate(correo, out var address)
            || !string.Equals(address.Address, correo, StringComparison.Ordinal)))
        {
            return Failure("DATOS_INVALIDOS", "El correo electrónico no tiene un formato válido.");
        }

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

        int? programaAcademicoId = null;
        if (programaAcademico is not null)
        {
            var programas = (await connection.QueryAsync<int>(new CommandDefinition(
                commandText: ProgramaSql,
                parameters: new { ProgramaAcademico = programaAcademico },
                transaction: transaction,
                cancellationToken: cancellationToken))).ToList();

            if (programas.Count == 0)
            {
                return Failure("PROGRAMA_NO_ENCONTRADO",
                    "No existe un programa académico con esa clave o nombre.");
            }

            if (programas.Count > 1)
            {
                return Failure("PROGRAMA_AMBIGUO",
                    "El programa académico es ambiguo. Indica su clave única.");
            }

            programaAcademicoId = programas[0];
        }

        if (telefono is not null)
        {
            var duplicado = await connection.QuerySingleAsync<bool>(new CommandDefinition(
                commandText: TelefonoSql,
                parameters: new { Telefono = telefono, Id = id.Value },
                transaction: transaction,
                cancellationToken: cancellationToken));

            if (duplicado)
            {
                return Failure("TELEFONO_DUPLICADO", "Ya existe otro estudiante con ese teléfono.");
            }
        }

        try
        {
            await connection.ExecuteAsync(new CommandDefinition(
                commandText: UpdateSql,
                parameters: new
                {
                    Id = id.Value,
                    Nombre = nombre,
                    Correo = correo,
                    Telefono = telefono,
                    ProgramaAcademicoId = programaAcademicoId
                },
                transaction: transaction,
                cancellationToken: cancellationToken));

            var estudiante = await connection.QuerySingleAsync<ConsultarEstudianteResponse>(
                new CommandDefinition(
                    commandText: EstudianteSql,
                    parameters: new { Id = id.Value },
                    transaction: transaction,
                    cancellationToken: cancellationToken));

            await transaction.CommitAsync(cancellationToken);

            return new ActualizarEstudianteResponse
            {
                Exito = true,
                Codigo = "ACTUALIZADO",
                Mensaje = "Estudiante actualizado correctamente.",
                Estudiante = estudiante
            };
        }
        catch (MySqlException exception) when (exception.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
        {
            // The unique constraint also protects against concurrent phone assignments.
            return Failure("TELEFONO_DUPLICADO", "Ya existe otro estudiante con ese teléfono.");
        }
        // Disposal rolls back any transaction that was not committed.
    }

    private static ActualizarEstudianteResponse Failure(string codigo, string mensaje) => new()
    {
        Exito = false,
        Codigo = codigo,
        Mensaje = mensaje
    };
}
