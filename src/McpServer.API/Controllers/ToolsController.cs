using McpServer.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace McpServer.API.Controllers;

[ApiController]
[Route("api/tools")]
public class ToolsController(IToolRegistryService registry) : ControllerBase
{
    [HttpGet("GetAllAsync")]
    public IActionResult GetAll()
    {
        return Ok(registry.GetAllTools());
    }

    [HttpGet("category/{category}")]
    public IActionResult GetByCategory(string category)
    {
        return Ok(registry.GetToolsByCategory(category));
    }
}
