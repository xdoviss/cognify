using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json; // Required for JSON deserialization

namespace cognify.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TypeRacerController : ControllerBase
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        [HttpGet]
        public async Task<ActionResult<string>> GetRandomText()
        {
            string randomText = await FetchChuckNorrisJoke();

            if (string.IsNullOrEmpty(randomText))
            {
                return BadRequest("Failed to fetch text");
            }

            return Ok(randomText);
        }

        private async Task<string> FetchChuckNorrisJoke()
        {
            try
            {
                // Request a random Chuck Norris joke from the API
                var response = await HttpClient.GetStringAsync("https://api.chucknorris.io/jokes/random");

                // Parse the response
                var jokeJson = JsonDocument.Parse(response);
                string joke = jokeJson.RootElement.GetProperty("value").GetString();

                return joke;
            }
            catch (HttpRequestException ex)
            {
                // Log error and return null in case of failure
                Console.WriteLine($"Error fetching joke: {ex.Message}");
                return null;
            }
        }
    }
}
