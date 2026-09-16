using MCP.Server.Shared.Configuration;
using MCP.Server.Shared.Database.MySql;
using MCP.Server.Shared.Database.MongoDb;
using MCP.Server.Features.ConsultarEstudiante;
using MCP.Server.Shared.Database.MongoDb.Repositories;
using MCP.Server.Features.ConsultarHistorial;
using MCP.Server.Features.ConsultarExpediente;
using MCP.Server.Features.ConsultarProfesor;
using MCP.Server.Features.ConsultarMateria;
using MCP.Server.Features.ConsultarGrupo;
using MCP.Server.Features.ConsultarHorarioEstudiante;
using MCP.Server.Features.ConsultarEvaluaciones;
using MCP.Server.Features.ConsultarCargaAcademica;
using MCP.Server.Features.RegistrarEstudiante;
using MCP.Server.Features.ActualizarEstudiante;
using MCP.Server.Features.EliminarEstudiante;
using MCP.Server.Features.RegistrarEvaluacion;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MySqlOptions>(
    builder.Configuration.GetSection(MySqlOptions.SectionName));

builder.Services.Configure<MongoDbOptions>(
    builder.Configuration.GetSection(MongoDbOptions.SectionName));

builder.Services.AddSingleton<MySqlConnectionFactory>();
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<ConsultarEstudianteService>();
builder.Services.AddScoped<ConsultarHistorialService>();
builder.Services.AddScoped<ConsultarExpedienteService>();
builder.Services.AddScoped<ConsultarProfesorService>();
builder.Services.AddScoped<ConsultarMateriaService>();
builder.Services.AddScoped<ConsultarGrupoService>();
builder.Services.AddScoped<ConsultarHorarioEstudianteService>();
builder.Services.AddScoped<ConsultarEvaluacionesService>();
builder.Services.AddScoped<ConsultarCargaAcademicaService>();
builder.Services.AddScoped<RegistrarEstudianteService>();
builder.Services.AddScoped<ActualizarEstudianteService>();
builder.Services.AddScoped<EliminarEstudianteService>();
builder.Services.AddScoped<RegistrarEvaluacionService>();

builder.Services
    .AddMcpServer()
    .WithHttpTransport(options =>
    {
        options.Stateless = true;
    })
    .WithTools<ConsultarEstudianteTool>()
    .WithTools<ConsultarHistorialTool>()
    .WithTools<ConsultarExpedienteTool>()
    .WithTools<ConsultarProfesorTool>()
    .WithTools<ConsultarMateriaTool>()
    .WithTools<ConsultarGrupoTool>()
    .WithTools<ConsultarHorarioEstudianteTool>()
    .WithTools<ConsultarEvaluacionesTool>()
    .WithTools<ConsultarCargaAcademicaTool>()
    .WithTools<RegistrarEstudianteTool>()
    .WithTools<ActualizarEstudianteTool>()
    .WithTools<EliminarEstudianteTool>()
    .WithTools<RegistrarEvaluacionTool>();
    
builder.Services.AddScoped<
    IHistorialAcademicoRepository,
    HistorialAcademicoRepository>();

builder.Services.AddScoped<IEvaluacionesRepository, EvaluacionesRepository>();
    


var app = builder.Build();

app.MapGet("/", () => "MCP.Server funcionando");

app.MapGet("/test/mysql", async (MySqlConnectionFactory factory) =>
{
    await using var connection = factory.CreateConnection();

    await connection.OpenAsync();

    return Results.Ok(new
    {
        message = "Conexión a MySQL exitosa"
    });
});

app.MapGet("/test/mongodb", async (
    MongoDbContext context,
    CancellationToken cancellationToken) =>
{
    await context.PingAsync(cancellationToken);

    return Results.Ok(new
    {
        message = "Conexión a MongoDB exitosa"
    });
});

app.MapGet(
    "/estudiantes/{boleta}",
    async (
        string boleta,
        ConsultarEstudianteService service,
        CancellationToken cancellationToken) =>
    {
        var estudiante = await service.ExecuteAsync(
            boleta,
            cancellationToken);

        return estudiante is null
            ? Results.NotFound(new
            {
                mensaje = "No se encontró el estudiante."
            })
            : Results.Ok(estudiante);
    });

app.MapMcp("/mcp");

app.Run();