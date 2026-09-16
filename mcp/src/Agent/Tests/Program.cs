using System.Net;
using System.Text;
using System.Text.Json;
using Agent.Features.Chat;
using Agent.Integrations.MCP;
using Agent.Integrations.Ollama;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

static JsonElement Json(string text) => JsonDocument.Parse(text).RootElement.Clone();
static void Check(bool condition, string label)
{
    if (!condition) throw new InvalidOperationException(label);
}

var cases = new (string Tool, string Message, string Arguments)[]
{
    ("consultar_estudiante", "Consulta la información general del estudiante con boleta 2025000003", "{\"boleta\":\"2025000003\"}"),
    ("consultar_historial", "Consulta el historial académico del estudiante con ID interno 3", "{\"estudianteId\":3}"),
    ("consultar_expediente", "Consulta el expediente completo de la boleta 2025000003", "{\"boleta\":\"2025000003\"}"),
    ("consultar_profesor", "Consulta al profesor número de empleado EMP001", "{\"numeroEmpleado\":\"EMP001\"}"),
    ("consultar_materia", "Consulta la materia con clave ISC202", "{\"clave\":\"ISC202\"}"),
    ("consultar_grupo", "Consulta el grupo 3CM1 del periodo 2026-2", "{\"claveGrupo\":\"3CM1\",\"periodo\":\"2026-2\"}"),
    ("consultar_horario_estudiante", "Consulta el horario del estudiante con boleta 2025000003 para el periodo 2026-2", "{\"boleta\":\"2025000003\",\"periodo\":\"2026-2\"}"),
    ("consultar_evaluaciones", "Consulta las evaluaciones detalladas del estudiante con ID interno 3 del periodo 2026-2", "{\"estudianteId\":3,\"periodo\":\"2026-2\"}"),
    ("consultar_carga_academica", "Consulta la carga académica activa de la boleta 2025000003 del periodo 2026-2 con sus evaluaciones disponibles", "{\"boleta\":\"2025000003\",\"periodo\":\"2026-2\"}")
};

// --live uses real tools/list and Qwen, but intercepts tools/call: it never queries the databases.
IReadOnlyList<McpToolInfo> tools;
McpService? liveMcp = null;
using var http = new HttpClient { BaseAddress = new Uri(Environment.GetEnvironmentVariable("OLLAMA_URL") ?? "http://localhost:11434"), Timeout = TimeSpan.FromMinutes(3) };
if (args.Contains("--live"))
{
    liveMcp = new McpService(Options.Create(new McpOptions
    {
        ServerUrl = Environment.GetEnvironmentVariable("MCP_URL") ?? "http://localhost:5000/mcp"
    }));
    tools = await liveMcp.ListToolsAsync();
    Check(tools.Count == 9, "El servidor debe publicar las nueve tools esperadas.");
}
else
{
    tools = cases.Select(c =>
    {
        var arguments = Json(c.Arguments);
        var properties = arguments.EnumerateObject().ToDictionary(p => p.Name,
            p => new { type = p.Value.ValueKind == JsonValueKind.Number ? "integer" : "string", description = p.Name });
        var schema = JsonSerializer.SerializeToElement(new { type = "object", properties, required = properties.Keys.ToArray() });
        return new McpToolInfo(c.Tool, c.Tool, schema);
    }).ToList();
}

var format = ChatDecisionSchema.Create(tools);
Check(format.GetProperty("anyOf").GetArrayLength() == 10, "Schemas y rama sin ejecución.");
Check(!tools[0].InputSchema.TryGetProperty("additionalProperties", out _), "No mutar inputSchema original.");

foreach (var c in cases)
{
    var stub = new StubMcp(tools);
    IOllamaService ollama = args.Contains("--live")
        ? new OllamaService(http, Options.Create(new OllamaOptions { Model = Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "qwen3:8b" }))
        : new StubOllama(JsonSerializer.Serialize(new { requiresTool = true, tool = c.Tool, arguments = Json(c.Arguments), response = (string?)null }));
    var chat = new ChatService(stub, ollama, NullLogger<ChatService>.Instance);
    var result = await chat.ExecuteAsync(new ChatRequest(c.Message));
    Check(stub.Calls == 1 && stub.LastTool == c.Tool, $"Selección {c.Tool}: {result.Response}");
    Check(stub.LastArguments!.Count == Json(c.Arguments).EnumerateObject().Count(), "Todos los argumentos.");
    foreach (var property in Json(c.Arguments).EnumerateObject())
    {
        Check(stub.LastArguments.TryGetValue(property.Name, out var actual) && actual is JsonElement,
            "Transporte como JsonElement sin conversiones.");
        Check(((JsonElement)stub.LastArguments[property.Name]!).GetRawText() == property.Value.GetRawText(), "Valor/tipo conservado.");
    }
    Check(result.ToolUsed == c.Tool, "Contrato ChatResponse.");
    Console.WriteLine($"PASS {c.Tool}");
}

if (args.Contains("--live"))
{
    foreach (var message in new[] { "Consulta el historial académico del estudiante", "Consulta las evaluaciones detalladas del estudiante con ID interno 3", "Consulta mi horario" })
    {
        var stub = new StubMcp(tools);
        var chat = new ChatService(stub, new OllamaService(http,
            Options.Create(new OllamaOptions { Model = Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "qwen3:8b" })), NullLogger<ChatService>.Instance);
        var result = await chat.ExecuteAsync(new ChatRequest(message));
        Check(stub.Calls == 0 && result.ToolUsed is null && !string.IsNullOrWhiteSpace(result.Response), "Debe solicitar datos faltantes.");
        Console.WriteLine($"PASS aclaración: {result.Response}");
    }
    await liveMcp!.DisposeAsync();
    return;
}

var historial = tools.Single(t => t.Name == "consultar_historial");
foreach (var invalid in new[] { "{}", "null", "[]", "{\"estudianteId\":null}", "{\"estudianteId\":\"3\"}", "{\"estudianteId\":3.5}", "{\"estudianteId\":2147483648}", "{\"estudianteId\":3,\"extra\":1}", "{\"EstudianteId\":3}", "{\"estudianteId\":3,\"estudianteId\":4}" })
{
    var stub = new StubMcp(tools);
    var decision = "{\"requiresTool\":true,\"tool\":\"consultar_historial\",\"arguments\":" + invalid + ",\"response\":null}";
    var chat = new ChatService(stub, new StubOllama(decision), NullLogger<ChatService>.Instance);
    var result = await chat.ExecuteAsync(new ChatRequest("Historial del estudiante ID 3"));
    Check(stub.Calls == 0 && result.ToolUsed is null, "Bloquear argumentos inválidos: " + invalid);
}
Check(ToolArgumentValidator.Validate(historial, Json("{\"estudianteId\":3}"), "Historial boleta 2025000003 periodo 2026-2").Count > 0,
    "No inferir ID de una parte de la boleta/periodo.");
var estudiante = tools.Single(t => t.Name == "consultar_estudiante");
Check(ToolArgumentValidator.Validate(estudiante, Json("{\"boleta\":\"00003\"}"), "Consulta boleta 00003").Count == 0, "Conservar ceros iniciales.");

foreach (var invalidDecision in new[] { "not json", "{}", "[]", "{\"requiresTool\":true,\"tool\":\"unknown\",\"arguments\":{},\"response\":null}", "{\"requiresTool\":false,\"tool\":null,\"arguments\":{},\"response\":null}" })
{
    var stub = new StubMcp(tools);
    var result = await new ChatService(stub, new StubOllama(invalidDecision), NullLogger<ChatService>.Instance)
        .ExecuteAsync(new ChatRequest("Consulta datos"));
    Check(stub.Calls == 0 && result.ToolUsed is null, "Controlar decisiones malformadas.");
}
var logger = new CaptureLogger();
var failingMcp = new StubMcp(tools) { Fail = true };
await new ChatService(failingMcp, new StubOllama("{\"requiresTool\":true,\"tool\":\"consultar_historial\",\"arguments\":{\"estudianteId\":3},\"response\":null}"), logger)
    .ExecuteAsync(new ChatRequest("Historial ID 3"));
Check(logger.Lines.Any(l => l.Contains("Decisión LLM")) && logger.Lines.Any(l => l.Contains("Ejecución MCP"))
    && logger.Lines.Any(l => l.Contains("Tiempo total backend")), "Mediciones incluso con errores.");
Check(!logger.Lines.Any(l => l.Contains("secret-password")), "No registrar mensajes de error con secretos.");

// Verify the real Ollama adapter's JSON request; final response must omit format.
var handler = new CaptureHandler();
using var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:11434") };
var adapter = new OllamaService(client, Options.Create(new OllamaOptions { Model = "qwen3:8b" }));
await adapter.ChatAsync([], responseFormat: format);
Check(handler.Body.GetProperty("format").GetProperty("anyOf").GetArrayLength() == 10, "Enviar schema en format.");
Check(!handler.Body.GetProperty("think").GetBoolean(), "No mezclar thinking con JSON.");
await adapter.ChatAsync([]);
Check(!handler.Body.TryGetProperty("format", out _), "Respuesta final en lenguaje natural.");
Console.WriteLine("PASS: nueve tools, argumentos faltantes/tipos/duplicados/inventados, salida malformada, logs, errores y contrato Ollama.");

sealed class StubMcp(IReadOnlyList<McpToolInfo> tools) : IMcpService
{
    public int Calls { get; private set; }
    public string? LastTool { get; private set; }
    public IReadOnlyDictionary<string, object?>? LastArguments { get; private set; }
    public bool Fail { get; init; }
    public Task<IReadOnlyList<McpToolInfo>> ListToolsAsync(CancellationToken cancellationToken = default) => Task.FromResult(tools);
    public Task<string> CallToolAsync(string toolName, IReadOnlyDictionary<string, object?> arguments, CancellationToken cancellationToken = default)
    {
        Calls++; LastTool = toolName; LastArguments = arguments;
        if (Fail) throw new InvalidOperationException("secret-password");
        return Task.FromResult("{\"mensaje\":\"Prueba de selección; no se consultaron bases de datos.\"}");
    }
}
sealed class StubOllama(string decision) : IOllamaService
{
    public Task<string> ChatAsync(IReadOnlyList<OllamaMessage> messages, CancellationToken cancellationToken = default, JsonElement? responseFormat = null)
    {
        if (responseFormat is not null && !messages[0].Content.Contains("inputSchema"))
            throw new InvalidOperationException("El prompt perdió inputSchema.");
        return Task.FromResult(responseFormat is null ? "Resultado de prueba" : decision);
    }
}
sealed class CaptureLogger : ILogger<ChatService>
{
    public List<string> Lines { get; } = [];
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    public bool IsEnabled(LogLevel logLevel) => true;
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) => Lines.Add(formatter(state, exception));
}
sealed class CaptureHandler : HttpMessageHandler
{
    public JsonElement Body { get; private set; }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using var document = JsonDocument.Parse(await request.Content!.ReadAsStringAsync(cancellationToken));
        Body = document.RootElement.Clone();
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"message\":{\"role\":\"assistant\",\"content\":\"ok\"},\"done\":true}", Encoding.UTF8, "application/json")
        };
    }
}
