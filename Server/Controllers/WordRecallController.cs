using System.Text.Json;

using cognify.Shared;
using Microsoft.AspNetCore.Mvc;

namespace cognify.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WordRecallController : ControllerBase
    {
        private static readonly List<string> SeenWords = new();
        private static readonly HttpClient HttpClient = new();
        private static WordRecallStatistics GameStatistics = new(GameState.NotStarted);
        private int sessionHighscore = 0;

        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            string word = await GetNewWord();

            while (SeenWords.Contains(word))
                word = await GetNewWord();

            SeenWords.Add(word);

            return word;
        }

        [HttpPost("start")]
        // ReSharper disable once UnusedMember.Global
        public ActionResult StartGame(GameState state = GameState.InProgress)
        {
            GameStatistics = new WordRecallStatistics(state);
            SeenWords.Clear();
            return Ok(GameStatistics);
        }

        [HttpPost("update-score")]
        // ReSharper disable once UnusedMember.Global
        public ActionResult UpdateScore([FromBody] int score)
        {
            if (GameStatistics.State == GameState.InProgress)
            {
                GameStatistics.Score += score;
                return Ok(GameStatistics);
            }
            return BadRequest("Game is not in progress.");
        }

        [HttpPost("update-health")]
        // ReSharper disable once UnusedMember.Global
        public ActionResult UpdateHealth([FromBody] int health)
        {
            if (GameStatistics.State == GameState.InProgress)
            {
                GameStatistics.Health = health;
                return Ok(GameStatistics);
            }
            return BadRequest("Game is not in progress.");
        }

        [HttpPost("update-state")]
        // ReSharper disable once UnusedMember.Global
        public ActionResult UpdateState([FromBody] GameState state)
        {
            GameStatistics.State = state;

            return Ok(GameStatistics);
        }

        [HttpGet("highscore")]
        // ReSharper disable once UnusedMember.Global
        public ActionResult<int> GetHighscore() => sessionHighscore;

        [HttpGet("is-finished")]
        // ReSharper disable once UnusedMember.Global
        public ActionResult<bool> IsGameFinished()
        {
            bool isFinished = GameStatistics.IsGameFinished();

            if (isFinished)
                UpdateHighscore();

            return Ok(isFinished);
        }

        private void UpdateHighscore()
        {
            if (GameStatistics.CompareTo(sessionHighscore) > 0)
                sessionHighscore = GameStatistics.Score;
        }

        private static async Task<string> GetNewWord()
        {
            var response = await HttpClient.GetStringAsync("https://random-word-api.vercel.app/api?words=1");

            using JsonDocument doc = JsonDocument.Parse(response);
            JsonElement root = doc.RootElement;
            return root[0].GetString();
        }
    }
}