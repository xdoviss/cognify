using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly ActivePlayerService _activePlayerService;

    public GameController(ActivePlayerService activePlayerService)
    {
        _activePlayerService = activePlayerService;
    }

    [HttpGet("activePlayerCount")]
    public IActionResult GetActivePlayerCount()
    {
        int count = _activePlayerService.GetActivePlayerCount();
        return Ok(count);
    }
}
