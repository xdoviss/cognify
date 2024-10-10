using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using cognify.Server.Models;
using cognify.Shared;


namespace cognify.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TypeRacerController : ControllerBase
    {
        private string filePath = "SampleTexts.txt";
        private GameResults gameResults = new GameResults();

        [HttpGet]
        public async Task<ActionResult<string>> GetRandomText()
        {
            string randomText = await LoadRandomTextFromFile();

            if (string.IsNullOrEmpty(randomText))
            {
                return BadRequest("Failed to load text from a file");
            }

            return Ok(randomText);
        }

        private async Task<string> LoadRandomTextFromFile()
        {
            try
            {
                // Read all lines from the file
                var lines = await System.IO.File.ReadAllLinesAsync(filePath);
                var random = new Random();

                if (lines.Length > 0)
                {
                    int index = random.Next(0, lines.Length);
                    return lines[index];
                }
                return null;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
                return null;
            }
        }
        [HttpPost("add-result")]
        public ActionResult AddGameResult([FromBody] GameResult result)
        {
            gameResults.AddResult(result);
            return Ok("Game result added successfully.");
        }

    }
}
