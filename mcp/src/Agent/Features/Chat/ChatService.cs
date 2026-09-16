using System.Text.Json;
using Agent.Integrations.MCP;
using Agent.Integrations.Ollama;

namespace Agent.Features.Chat;

public sealed class ChatService : IChatService
{
    private readonly IMcpService _mcpService;
    private readonly IOllamaService _ollamaService;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ChatService(
        IMcpService mcpService,
        IOllamaService ollamaService)
    {
        _mcpService = mcpService;
        _ollamaService = ollamaService;
    }

    public async Task<ChatResponse> ExecuteAsync(
        ChatRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            throw new ArgumentException(
                "El mensaje no puede estar vacío.",
                nameof(request));
        }

        // 1. Descubrir dinámicamente las tools del MCP Server.
        var availableTools = await _mcpService.ListToolsAsync(
            cancellationToken);

        // 2. Convertir las tools a JSON para mostrárselas al modelo.
        var toolsJson = JsonSerializer.Serialize(
            availableTools,
            JsonOptions);

        // 3. Pedirle a Qwen que decida si necesita una tool.
        var decisionMessages = BuildDecisionMessages(
            request.Message,
            toolsJson);

        var decisionJson = await _ollamaService.ChatAsync(
            decisionMessages,
            cancellationToken);

        // Algunos modelos envuelven el JSON en ```json ... ```.
        var cleanDecisionJson = CleanJsonResponse(decisionJson);

        var decision = JsonSerializer.Deserialize<ChatToolDecision>(
            cleanDecisionJson,
            JsonOptions);

        if (decision is null)
        {
            throw new InvalidOperationException(
                "No fue posible interpretar la decisión del modelo.");
        }

        // 4. Si no requiere una tool, regresamos la respuesta directa.
        if (!decision.RequiresTool)
        {
            return new ChatResponse(
                decision.Response
                    ?? "No fue posible generar una respuesta.",
                null);
        }

        // 5. Validar que Qwen haya seleccionado una tool real.
        if (string.IsNullOrWhiteSpace(decision.Tool))
        {
            throw new InvalidOperationException(
                "El modelo indicó que requiere una tool, "
                + "pero no especificó cuál.");
        }

        var selectedTool = availableTools.FirstOrDefault(
            tool => string.Equals(
                tool.Name,
                decision.Tool,
                StringComparison.OrdinalIgnoreCase));

        if (selectedTool is null)
        {
            throw new InvalidOperationException(
                $"El modelo seleccionó una tool inexistente: "
                + $"{decision.Tool}");
        }

        // 6. Ejecutar la tool mediante el cliente MCP.
        var toolResult = await _mcpService.CallToolAsync(
            selectedTool.Name,
            decision.Arguments,
            cancellationToken);

        // 7. Pedirle a Qwen que transforme el resultado técnico
        //    en una respuesta entendible para el usuario.
        var finalMessages = BuildFinalResponseMessages(
            request.Message,
            selectedTool.Name,
            toolResult);

        var finalResponse = await _ollamaService.ChatAsync(
            finalMessages,
            cancellationToken);

        return new ChatResponse(
            finalResponse,
            selectedTool.Name);
    }

    private static IReadOnlyList<OllamaMessage> BuildDecisionMessages(
        string userMessage,
        string toolsJson)
    {
        return
        [
            new OllamaMessage
            {
                Role = "system",
                Content =
                    """
                    Eres un agente académico.

                    Debes decidir si la solicitud del usuario requiere
                    ejecutar una de las herramientas disponibles.

                    Responde únicamente con JSON válido, sin Markdown,
                    explicaciones ni bloques de código.

                    Usa exactamente esta estructura:

                    {
                      "requiresTool": true o false,
                      "tool": "nombre de la herramienta o null",
                      "arguments": {},
                      "response": "respuesta directa o null"
                    }

                    Reglas:

                    - Solo puedes seleccionar herramientas de la lista.
                    - No inventes herramientas.
                    - Si necesitas una herramienta, coloca sus argumentos.
                    - Si no necesitas herramienta, responde directamente
                      en el campo "response".
                    - Cuando requiresTool sea true, response debe ser null.
                    - Cuando requiresTool sea false, tool debe ser null
                      y arguments debe ser un objeto vacío.
                    """
            },
            new OllamaMessage
            {
                Role = "user",
                Content =
                    $"""
                    Herramientas disponibles:

                    {toolsJson}

                    Solicitud del usuario:

                    {userMessage}
                    """
            }
        ];
    }

    private static IReadOnlyList<OllamaMessage>
        BuildFinalResponseMessages(
            string userMessage,
            string toolName,
            string toolResult)
    {
        return
        [
            new OllamaMessage
            {
                Role = "system",
                Content =
                    """
                    Eres un asistente académico.

                    Responde al usuario utilizando únicamente la información
                    obtenida mediante la herramienta.

                    No inventes datos.
                    No menciones detalles internos, bases de datos,
                    JSON-RPC, MCP, SQL, MongoDB ni nombres de servicios.
                    No muestres código ni información de configuración.
                    Presenta la información de manera clara y natural.
                    """
            },
            new OllamaMessage
            {
                Role = "user",
                Content =
                    $"""
                    Solicitud original:

                    {userMessage}

                    Herramienta ejecutada:

                    {toolName}

                    Resultado obtenido:

                    {toolResult}
                    """
            }
        ];
    }

    private static string CleanJsonResponse(string response)
    {
        var cleanResponse = response.Trim();

        if (cleanResponse.StartsWith("```json"))
        {
            cleanResponse = cleanResponse["```json".Length..];
        }
        else if (cleanResponse.StartsWith("```"))
        {
            cleanResponse = cleanResponse["```".Length..];
        }

        if (cleanResponse.EndsWith("```"))
        {
            cleanResponse = cleanResponse[..^3];
        }

        return cleanResponse.Trim();
    }
}