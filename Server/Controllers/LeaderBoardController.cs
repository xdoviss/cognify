using Microsoft.AspNetCore.Mvc;
using cognify.Shared;

namespace cognify.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaderBoardController : ControllerBase
    {
        private readonly GameResultService _gameResultService;

        public LeaderBoardController(GameResultService gameResultService)
        {
            _gameResultService = gameResultService;
        }

        [HttpGet("results")]
        public ActionResult<List<GameResult>> GetGameResults()
        {
            var results = _gameResultService.GetResults();
            return Ok(results);
        }
    }
}
