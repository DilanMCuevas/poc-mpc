using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Agent.Integrations.MCP;

namespace Agent.Features.Chat;

public static class ToolArgumentValidator
{
    // The current MCP schemas contain flat string/integer parameters.
    // Unknown types fail closed instead of silently bypassing validation.
    public static IReadOnlyList<string> Validate(
        McpToolInfo tool, JsonElement arguments, string userMessage)
    {
        var errors = new List<string>();
        if (arguments.ValueKind != JsonValueKind.Object)
        {
            return ["Los argumentos deben ser un objeto JSON."];
        }

        var schema = tool.InputSchema;
        if (!schema.TryGetProperty("properties", out var properties))
        {
            return ["No se pudo validar el esquema de parámetros de la herramienta."];
        }

        if (schema.TryGetProperty("required", out var required))
        {
            foreach (var parameter in required.EnumerateArray())
            {
                var name = parameter.GetString()!;
                if (!arguments.TryGetProperty(name, out var value)
                    || value.ValueKind == JsonValueKind.Null
                    || (value.ValueKind == JsonValueKind.String
                        && string.IsNullOrWhiteSpace(value.GetString())))
                {
                    var description = properties.TryGetProperty(name, out var definition)
                        && definition.TryGetProperty("description", out var text)
                        ? $" ({text.GetString()})" : string.Empty;
                    errors.Add($"Falta el dato requerido {name}{description}.");
                }
            }
        }

        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var argument in arguments.EnumerateObject())
        {
            if (!seen.Add(argument.Name))
            {
                errors.Add("El modelo repitió un parámetro.");
                continue;
            }

            if (!properties.TryGetProperty(argument.Name, out var definition))
            {
                errors.Add("El modelo incluyó un parámetro que no pertenece al esquema.");
                continue;
            }

            var type = definition.TryGetProperty("type", out var typeValue)
                && typeValue.ValueKind == JsonValueKind.String ? typeValue.GetString() : null;
            var validType = type switch
            {
                "string" => argument.Value.ValueKind == JsonValueKind.String
                    && !string.IsNullOrWhiteSpace(argument.Value.GetString()),
                "integer" => argument.Value.ValueKind == JsonValueKind.Number
                    && argument.Value.TryGetInt32(out _),
                _ => false
            };

            if (!validType)
            {
                errors.Add($"El dato {argument.Name} debe tener el tipo {type ?? "declarado"} válido.");
                continue;
            }

            var literal = type == "string" ? argument.Value.GetString()!
                : argument.Value.GetInt32().ToString(CultureInfo.InvariantCulture);
            // Require a literal supplied by the user: no guessed IDs, defaults or example values.
            var pattern = @"(?<![\p{L}\p{N}_-])" + Regex.Escape(literal) + @"(?![\p{L}\p{N}_-])";
            if (!Regex.IsMatch(userMessage, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
                    TimeSpan.FromMilliseconds(100)))
            {
                errors.Add($"Indica explícitamente el dato {argument.Name} en tu solicitud.");
            }
        }

        return errors;
    }
}
