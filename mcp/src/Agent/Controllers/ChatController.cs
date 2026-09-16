using Agent.Features.Chat;
using Microsoft.AspNetCore.Mvc;

namespace Agent.Controllers;

[ApiController]
[Route("chat")]
public sealed class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost]
    public async Task<ActionResult<ChatResponse>> SendAsync(
        [FromBody] ChatRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _chatService.ExecuteAsync(
            request,
            cancellationToken);

        return Ok(response);
    }
}