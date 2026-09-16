namespace Agent.Features.Chat;

public interface IChatService
{
    Task<ChatResponse> ExecuteAsync(
        ChatRequest request,
        CancellationToken cancellationToken = default);
}