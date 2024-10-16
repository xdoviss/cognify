using Microsoft.AspNetCore.Mvc;
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

        public TypeRacerController(TextLoaderService textLoaderService, GameResultService gameResultService)
        {
            _textLoaderService = textLoaderService;
            _gameResultService = gameResultService;
        }


        [HttpGet]
        public async Task<ActionResult<string>> GetRandomText()
        {
            string randomText = await _textLoaderService.LoadRandomTextFromFileAsync();

            if (string.IsNullOrEmpty(randomText))
            {
                return BadRequest("Failed to load text from a file");
            }

            return Ok(randomText);
        }

        


    }
}
