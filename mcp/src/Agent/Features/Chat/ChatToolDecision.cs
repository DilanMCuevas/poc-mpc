using System.Text.Json;
using System.Text.Json.Serialization;

namespace Agent.Features.Chat;

public sealed record ChatToolDecision(
    [property: JsonRequired] bool RequiresTool,
    [property: JsonRequired] string? Tool,
    [property: JsonRequired] JsonElement Arguments,
    [property: JsonRequired] string? Response);
