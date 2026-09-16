using System.Net.Mail;
using Dapper;
using MCP.Server.Features.ConsultarEstudiante;
using MCP.Server.Shared.Database.MySql;
using MySqlConnector;

namespace MCP.Server.Features.RegistrarEstudiante;

public sealed class RegistrarEstudianteService
{
    private readonly MySqlConnectionFactory _connectionFactory;

    private const string ProgramaSql = """
        SELECT id
        FROM programas_academicos
        WHERE clave = @ProgramaAcademico OR nombre = @ProgramaAcademico
        LIMIT 2
        FOR SHARE;
        """;

    private const string DuplicadosSql = """
        SELECT
            EXISTS(SELECT 1 FROM estudiantes WHERE boleta = @Boleta) AS BoletaDuplicada,
            EXISTS(SELECT 1 FROM estudiantes WHERE telefono = @Telefono) AS TelefonoDuplicado;
        """;

    private const string InsertSql = """
        INSERT INTO estudiantes
            (programa_academico_id, boleta, nombre, correo, telefono)
        VALUES
            (@ProgramaAcademicoId, @Boleta, @Nombre, @Correo, @Telefono);
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
        WHERE e.id = LAST_INSERT_ID();
        """;

    public RegistrarEstudianteService(MySqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<RegistrarEstudianteResponse> ExecuteAsync(
        string boleta,
        string nombre,
        string correo,
        string telefono,
        string programaAcademico,
        CancellationToken cancellationToken = default)
    {
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
            if (string.IsNullOrWhiteSpace(input.Value))
            {
                return Failure($"El dato {input.Name} es obligatorio.");
            }

            if (input.Value.Trim().Length > input.MaxLength)
            {
                return Failure($"El dato {input.Name} no puede exceder {input.MaxLength} caracteres.");
            }

            if (input.Value.Any(char.IsControl))
            {
                return Failure($"El dato {input.Name} contiene caracteres de control no válidos.");
            }
        }

        boleta = boleta.Trim();
        nombre = nombre.Trim();
        correo = correo.Trim();
        telefono = telefono.Trim();
        programaAcademico = programaAcademico.Trim();

        if (!MailAddress.TryCreate(correo, out var address)
            || !string.Equals(address.Address, correo, StringComparison.Ordinal))
        {
            return Failure("El correo electrónico no tiene un formato válido.");
        }

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        var programas = (await connection.QueryAsync<int>(new CommandDefinition(
            commandText: ProgramaSql,
            parameters: new { ProgramaAcademico = programaAcademico },
            transaction: transaction,
            cancellationToken: cancellationToken))).ToList();

        if (programas.Count == 0)
        {
            return Failure("No existe un programa académico con esa clave o nombre.");
        }

        if (programas.Count > 1)
        {
            return Failure("El programa académico es ambiguo. Indica su clave única.");
        }

        var duplicados = await connection.QuerySingleAsync<Duplicados>(new CommandDefinition(
            commandText: DuplicadosSql,
            parameters: new { Boleta = boleta, Telefono = telefono },
            transaction: transaction,
            cancellationToken: cancellationToken));

        if (duplicados.BoletaDuplicada)
        {
            return Failure("Ya existe un estudiante con esa boleta.");
        }

        if (duplicados.TelefonoDuplicado)
        {
            return Failure("Ya existe un estudiante con ese teléfono.");
        }

        try
        {
            await connection.ExecuteAsync(new CommandDefinition(
                commandText: InsertSql,
                parameters: new
                {
                    ProgramaAcademicoId = programas[0],
                    Boleta = boleta,
                    Nombre = nombre,
                    Correo = correo,
                    Telefono = telefono
                },
                transaction: transaction,
                cancellationToken: cancellationToken));

            var estudiante = await connection.QuerySingleAsync<ConsultarEstudianteResponse>(
                new CommandDefinition(
                    commandText: EstudianteSql,
                    transaction: transaction,
                    cancellationToken: cancellationToken));

            await transaction.CommitAsync(cancellationToken);

            return new RegistrarEstudianteResponse
            {
                Exito = true,
                Mensaje = "Estudiante registrado correctamente.",
                Estudiante = estudiante
            };
        }
        catch (MySqlException exception) when (exception.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
        {
            // Unique constraints also protect concurrent inserts after the pre-check.
            return Failure("Ya existe un estudiante con esa boleta o teléfono.");
        }
        // Disposal rolls back any transaction that was not committed, including cancellations.
    }

    private static RegistrarEstudianteResponse Failure(string mensaje) => new()
    {
        Exito = false,
        Mensaje = mensaje
    };

    private sealed class Duplicados
    {
        public bool BoletaDuplicada { get; init; }
        public bool TelefonoDuplicado { get; init; }
    }
}
