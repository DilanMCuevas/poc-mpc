namespace Agent.Features.Chat;

public sealed record ChatResponse(
    string Response,
    string? ToolUsed);