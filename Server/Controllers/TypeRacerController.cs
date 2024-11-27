using Microsoft.AspNetCore.Mvc;
using cognify.Server.Services;
using System.Net.Http;
using System.Threading.Tasks;
using cognify.Shared;
using System.IO;

namespace cognify.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TypeRacerController : ControllerBase
    {
        private readonly TextLoaderService _textLoaderService;
        private readonly GameResultService _gameResultService;
        private readonly ActivePlayerService _activePlayerService;

        public TypeRacerController(TextLoaderService textLoaderService, GameResultService gameResultService, ActivePlayerService activePlayerService)
        {
            _textLoaderService = textLoaderService;
            _gameResultService = gameResultService;
            _activePlayerService = activePlayerService;
        }


        [HttpGet]
        public async Task<ActionResult<string>> GetProcessedText()
        {
            string randomText = await _textLoaderService.LoadRandomTextFromFileAsync();

            if (string.IsNullOrEmpty(randomText))
            {
                return BadRequest("Failed to load text from a file");
            }
            try
            {
                // Using the TextProcessor to check through the text
                var processor = new TextProcessor<TextMetadata>();
                TextMetadata metadata = processor.ProcessText(
                    text => new TextMetadata { OriginalText = text, WordCount = text.Split(' ').Length },
                    randomText);
                return Ok(metadata.OriginalText);
            }
            catch(ArgumentException ex)
            {
                return BadRequest($"Validation failed: {ex.Message}");
            }

            return Ok(randomText);
        }
        [HttpPost("startGame")]
        public IActionResult StartGame([FromBody] string userId)
        {
            // Adding a user to active players
            _activePlayerService.AddPlayer(userId);
            Console.WriteLine($"User created with ID: {userId}");
            return Ok();
        }

        [HttpPost("finishGame")]
        public IActionResult FinishGame([FromBody] string userId)
        {
            // Remove user from active players
            _activePlayerService.RemovePlayer(userId);
            Console.WriteLine($"User removed with ID: {userId}");
            return Ok();
        }
    }
}
