using System.Text.Json;
using System.Text.Json.Nodes;
using Agent.Integrations.MCP;

namespace Agent.Features.Chat;

public static class ChatDecisionSchema
{
    public static JsonElement Create(IReadOnlyList<McpToolInfo> tools)
    {
        var alternatives = new JsonArray();
        foreach (var tool in tools)
        {
            var arguments = JsonNode.Parse(tool.InputSchema.GetRawText())!.AsObject();
            // Only parameters advertised by MCP may be generated.
            arguments["additionalProperties"] = false;
            alternatives.Add(Branch(true,
                new JsonObject { ["const"] = tool.Name }, arguments,
                new JsonObject { ["type"] = "null" }));
        }

        // Missing information must remain a valid alternative to inventing required values.
        alternatives.Add(Branch(false, new JsonObject { ["type"] = "null" },
            new JsonObject
            {
                ["type"] = "object",
                ["properties"] = new JsonObject(),
                ["additionalProperties"] = false
            },
            new JsonObject { ["type"] = "string", ["minLength"] = 1 }));

        return JsonSerializer.SerializeToElement(new JsonObject { ["anyOf"] = alternatives });
    }

    private static JsonObject Branch(bool requiresTool, JsonObject tool,
        JsonObject arguments, JsonObject response) => new()
    {
        ["type"] = "object",
        ["properties"] = new JsonObject
        {
            ["requiresTool"] = new JsonObject { ["const"] = requiresTool },
            ["tool"] = tool,
            ["arguments"] = arguments,
            ["response"] = response
        },
        ["required"] = new JsonArray("requiresTool", "tool", "arguments", "response"),
        ["additionalProperties"] = false
    };
}
