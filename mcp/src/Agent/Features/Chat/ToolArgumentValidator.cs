using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Agent.Integrations.MCP;

namespace Agent.Features.Chat;

public static class ToolArgumentValidator
{
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
                // Required constrains presence; the property's schema determines nullability.
                if (!arguments.TryGetProperty(name, out _))
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

            if (!ArgumentSchemaMatcher.Matches(argument.Value, definition, schema))
            {
                errors.Add($"El dato {argument.Name} no cumple el esquema de tipos y restricciones declarado.");
                continue;
            }

            if (!ContainsUserValues(argument.Value, userMessage))
            {
                errors.Add($"Indica explícitamente el dato {argument.Name} en tu solicitud.");
            }
        }

        if (errors.Count == 0 && !ArgumentSchemaMatcher.Matches(arguments, schema, schema))
        {
            errors.Add("Los argumentos no cumplen las restricciones del esquema MCP.");
        }

        return errors;
    }

    private static bool ContainsUserValues(JsonElement value, string userMessage)
    {
        // Null is accepted only after schema validation, never coerced to a string or default.
        if (value.ValueKind == JsonValueKind.Null) return true;
        if (value.ValueKind == JsonValueKind.Array)
            return value.EnumerateArray().All(item => ContainsUserValues(item, userMessage));
        if (value.ValueKind == JsonValueKind.Object)
            return value.EnumerateObject().All(item => ContainsUserValues(item.Value, userMessage));

        var literal = value.ValueKind switch
        {
            JsonValueKind.String => value.GetString()!,
            JsonValueKind.Number when value.TryGetDecimal(out var number) =>
                number.ToString("G29", CultureInfo.InvariantCulture),
            _ => value.GetRawText()
        };
        if (string.IsNullOrWhiteSpace(literal)) return false;

        // Preserve the existing evidence check without depending on how 'type' is encoded.
        var pattern = @"(?<![\p{L}\p{N}_-])" + Regex.Escape(literal) + @"(?![\p{L}\p{N}_-])";
        return Regex.IsMatch(userMessage, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
            TimeSpan.FromMilliseconds(100));
    }
}
