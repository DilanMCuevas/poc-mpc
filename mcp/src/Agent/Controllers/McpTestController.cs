using Agent.Integrations.MCP;
using Microsoft.AspNetCore.Mvc;

namespace Agent.Controllers;

[ApiController]
[Route("test/mcp")]
public sealed class McpTestController : ControllerBase
{
    private readonly IMcpService _mcpService;

    public McpTestController(IMcpService mcpService)
    {
        _mcpService = mcpService;
    }

    [HttpGet("tools")]
    public async Task<ActionResult<IReadOnlyList<McpToolInfo>>> GetToolsAsync(
        CancellationToken cancellationToken)
    {
        var tools = await _mcpService.ListToolsAsync(cancellationToken);

        return Ok(tools);
    }
}
