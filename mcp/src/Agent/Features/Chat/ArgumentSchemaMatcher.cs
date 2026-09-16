using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Agent.Features.Chat;

// JSON Schema assertions used by MCP arguments. Unsupported assertions fail closed.
// Metadata (including default) never supplies missing argument values.
internal static class ArgumentSchemaMatcher
{
    private static readonly HashSet<string> SupportedKeywords = new(StringComparer.Ordinal)
    {
        "$schema", "$id", "$defs", "definitions", "$ref", "$comment",
        "title", "description", "default", "examples", "deprecated", "readOnly", "writeOnly", "format",
        "type", "anyOf", "oneOf", "allOf", "not", "enum", "const",
        "properties", "required", "additionalProperties", "minProperties", "maxProperties",
        "items", "prefixItems", "minItems", "maxItems", "uniqueItems",
        "minLength", "maxLength", "pattern",
        "minimum", "maximum", "exclusiveMinimum", "exclusiveMaximum", "multipleOf"
    };

    public static bool Matches(JsonElement value, JsonElement schema, JsonElement root)
    {
        try
        {
            return SupportsSchema(schema, root, 0) && Match(value, schema, root, 0);
        }
        catch (Exception exception) when (exception is InvalidOperationException or FormatException
            or OverflowException or ArgumentException or RegexMatchTimeoutException)
        {
            return false;
        }
    }

    private static bool SupportsSchema(JsonElement schema, JsonElement root, int depth)
    {
        if (depth > 64) return false;
        if (schema.ValueKind is JsonValueKind.True or JsonValueKind.False) return true;
        if (schema.ValueKind != JsonValueKind.Object
            || schema.EnumerateObject().Any(p => !SupportedKeywords.Contains(p.Name))) return false;
        foreach (var property in schema.EnumerateObject())
        {
            switch (property.Name)
            {
                case "$ref":
                    if (!TryResolve(property.Value.GetString(), root, out var target)
                        || !SupportsSchema(target, root, depth + 1)) return false;
                    break;
                case "anyOf":
                case "oneOf":
                case "allOf":
                case "prefixItems":
                    if (!property.Value.EnumerateArray().All(s => SupportsSchema(s, root, depth + 1))) return false;
                    break;
                case "not":
                case "items":
                case "additionalProperties":
                    if (!SupportsSchema(property.Value, root, depth + 1)) return false;
                    break;
                case "properties":
                    if (!property.Value.EnumerateObject().All(p => SupportsSchema(p.Value, root, depth + 1))) return false;
                    break;
            }
        }
        return true;
    }

    private static bool TryResolve(string? path, JsonElement root, out JsonElement target)
    {
        target = root;
        if (path is null || (path != "#" && !path.StartsWith("#/", StringComparison.Ordinal))) return false;
        foreach (var segment in path == "#" ? [] : path[2..].Split('/'))
        {
            var key = Uri.UnescapeDataString(segment).Replace("~1", "/").Replace("~0", "~");
            if (target.ValueKind != JsonValueKind.Object || !target.TryGetProperty(key, out target)) return false;
        }
        return true;
    }

    private static bool Match(JsonElement value, JsonElement schema, JsonElement root, int depth)
    {
        if (depth > 64) return false;
        if (schema.ValueKind is JsonValueKind.True or JsonValueKind.False) return schema.GetBoolean();
        if (schema.ValueKind != JsonValueKind.Object
            || schema.EnumerateObject().Any(p => !SupportedKeywords.Contains(p.Name))) return false;

        if (schema.TryGetProperty("$ref", out var reference))
        {
            if (!TryResolve(reference.GetString(), root, out var target)
                || !Match(value, target, root, depth + 1)) return false;
        }

        if (schema.TryGetProperty("type", out var type))
        {
            var accepted = type.ValueKind switch
            {
                JsonValueKind.String => HasType(value, type.GetString()),
                JsonValueKind.Array => type.EnumerateArray().Any(t => t.ValueKind == JsonValueKind.String
                    && HasType(value, t.GetString())),
                _ => false
            };
            if (!accepted) return false;
        }

        foreach (var keyword in new[] { "anyOf", "oneOf", "allOf" })
        {
            if (!schema.TryGetProperty(keyword, out var branches)) continue;
            if (branches.ValueKind != JsonValueKind.Array || branches.GetArrayLength() == 0) return false;
            var matches = branches.EnumerateArray().Count(branch => Match(value, branch, root, depth + 1));
            if (keyword == "anyOf" && matches == 0
                || keyword == "oneOf" && matches != 1
                || keyword == "allOf" && matches != branches.GetArrayLength()) return false;
        }
        if (schema.TryGetProperty("not", out var excluded) && Match(value, excluded, root, depth + 1)) return false;
        if (schema.TryGetProperty("const", out var constant) && !JsonElement.DeepEquals(value, constant)) return false;
        if (schema.TryGetProperty("enum", out var choices)
            && !choices.EnumerateArray().Any(choice => JsonElement.DeepEquals(value, choice))) return false;

        switch (value.ValueKind)
        {
            case JsonValueKind.String:
                var text = value.GetString()!;
                if (!WithinSize(text.EnumerateRunes().Count(), schema, "minLength", "maxLength")) return false;
                if (schema.TryGetProperty("pattern", out var pattern)
                    && !Regex.IsMatch(text, pattern.GetString()!, RegexOptions.CultureInvariant,
                        TimeSpan.FromMilliseconds(100))) return false;
                break;
            case JsonValueKind.Number:
                foreach (var keyword in new[] { "minimum", "maximum", "exclusiveMinimum", "exclusiveMaximum", "multipleOf" })
                {
                    if (!schema.TryGetProperty(keyword, out var limit)) continue;
                    if (!value.TryGetDecimal(out var number) || !limit.TryGetDecimal(out var bound)) return false;
                    if (keyword == "minimum" && number < bound
                        || keyword == "maximum" && number > bound
                        || keyword == "exclusiveMinimum" && number <= bound
                        || keyword == "exclusiveMaximum" && number >= bound
                        || keyword == "multipleOf" && (bound <= 0 || number % bound != 0)) return false;
                }
                break;
            case JsonValueKind.Object:
                var fields = value.EnumerateObject().ToArray();
                if (fields.Select(p => p.Name).Distinct(StringComparer.Ordinal).Count() != fields.Length
                    || !WithinSize(fields.Length, schema, "minProperties", "maxProperties")) return false;
                if (schema.TryGetProperty("required", out var required)
                    && required.EnumerateArray().Any(name => !value.TryGetProperty(name.GetString()!, out _))) return false;
                foreach (var field in fields)
                {
                    if (schema.TryGetProperty("properties", out var properties)
                        && properties.TryGetProperty(field.Name, out var definition))
                    {
                        if (!Match(field.Value, definition, root, depth + 1)) return false;
                    }
                    else if (schema.TryGetProperty("additionalProperties", out var additional)
                        && !Match(field.Value, additional, root, depth + 1)) return false;
                }
                break;
            case JsonValueKind.Array:
                var items = value.EnumerateArray().ToArray();
                if (!WithinSize(items.Length, schema, "minItems", "maxItems")) return false;
                var prefixLength = schema.TryGetProperty("prefixItems", out var prefix) ? prefix.GetArrayLength() : 0;
                for (var index = 0; index < items.Length; index++)
                {
                    if (index < prefixLength)
                    {
                        if (!Match(items[index], prefix[index], root, depth + 1)) return false;
                    }
                    else if (schema.TryGetProperty("items", out var itemSchema)
                        && !Match(items[index], itemSchema, root, depth + 1)) return false;
                    if (schema.TryGetProperty("uniqueItems", out var unique) && unique.GetBoolean()
                        && items.Take(index).Any(other => JsonElement.DeepEquals(items[index], other))) return false;
                }
                break;
        }
        return true;
    }

    private static bool HasType(JsonElement value, string? type) => type switch
    {
        "string" => value.ValueKind == JsonValueKind.String,
        "null" => value.ValueKind == JsonValueKind.Null,
        "boolean" => value.ValueKind is JsonValueKind.True or JsonValueKind.False,
        "number" => value.ValueKind == JsonValueKind.Number,
        "integer" => value.ValueKind == JsonValueKind.Number && IsInteger(value.GetRawText()),
        "object" => value.ValueKind == JsonValueKind.Object,
        "array" => value.ValueKind == JsonValueKind.Array,
        _ => false
    };

    private static bool IsInteger(string raw)
    {
        // Inspect the decimal representation without rounding a fractional JSON number.
        var exponentIndex = raw.IndexOfAny(['e', 'E']);
        var mantissa = exponentIndex < 0 ? raw : raw[..exponentIndex];
        long exponent = 0;
        if (exponentIndex >= 0 && !long.TryParse(raw.AsSpan(exponentIndex + 1),
                NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out exponent)) return false;
        var point = mantissa.IndexOf('.');
        var fractionalDigits = point < 0 ? 0 : mantissa.Length - point - 1;
        var digits = mantissa.Replace(".", "").TrimStart('-');
        var significantDigits = digits.TrimEnd('0');
        if (significantDigits.Length == 0) return true;
        return exponent >= fractionalDigits - (digits.Length - significantDigits.Length);
    }

    private static bool WithinSize(int size, JsonElement schema, string minimum, string maximum) =>
        (!schema.TryGetProperty(minimum, out var min) || size >= min.GetInt32())
        && (!schema.TryGetProperty(maximum, out var max) || size <= max.GetInt32());
}
