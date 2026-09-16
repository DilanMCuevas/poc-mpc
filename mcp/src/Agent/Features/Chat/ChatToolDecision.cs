namespace Agent.Features.Chat;

public sealed record ChatToolDecision(
    bool RequiresTool,
    string? Tool,
    IReadOnlyDictionary<string, object?> Arguments,
    string? Response);