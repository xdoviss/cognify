using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using cognify.Server.Models;
using cognify.Shared;
using System.IO;



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
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (StreamReader reader = new StreamReader(fs))
                {
                    // Read all lines from the file stream, split them and get a random line from a file
                    var lines = await reader.ReadToEndAsync();
                    var lineArray = lines.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    var random = new Random();

                    if (lineArray.Length > 0)
                    {
                        int index = random.Next(0, lineArray.Length);
                        return lineArray[index];
                    }
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
