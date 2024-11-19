using Microsoft.AspNetCore.Mvc;
using cognify.Shared;
using cognify.Server.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public async Task<ActionResult<List<GameResult>>> GetGameResults()
        {
            var results = await _gameResultService.GetResultsAsync();
            return Ok(results);
        }

        [HttpGet("results/{id}")]
        public async Task<ActionResult<GameResult>> GetGameResultById(int id)
        {
            var result = await _gameResultService.GetResultByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost("results")]
        public async Task<ActionResult<GameResult>> AddGameResult([FromBody] GameResult gameResult)
        {
            await _gameResultService.AddResultAsync(gameResult);
            return CreatedAtAction(nameof(GetGameResultById), new { id = gameResult.Id }, gameResult);
        }
        [HttpPut("results/{id}")]
        public async Task<IActionResult> UpdateGameResult(int id, GameResult updatedResult)
        {
            if (id != updatedResult.Id)
            {
                return BadRequest("ID mismatch between URL and payload");
            }

            await _gameResultService.UpdateResultAsync(updatedResult);
            return NoContent();
        }
        [HttpDelete("results/{id}")]
        public async Task<IActionResult> DeleteGameResult(int id)
        {
            await _gameResultService.DeleteResultAsync(id);
            return NoContent();
        }
    }
}

