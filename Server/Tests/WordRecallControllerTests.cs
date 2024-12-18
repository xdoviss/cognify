using cognify.Server.Controllers;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using cognify.Shared;
using System.Threading.Tasks;

namespace cognify.Server.Tests
{
    public class WordRecall_UnitTests
    {
        private readonly WordRecallController _controller = new();

        [Fact]
        public async Task Test_ReturnsNewWord()
        {
            var result = await _controller.Get();

            Assert.IsType<ActionResult<string>>(result);
        }

        [Fact]
        public void Test_StartGame()
        {
            var result = _controller.StartGame();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Test_UpdateScore()
        {
            _controller.StartGame();
            int score = new Random().Next(0, 20);

            var result = _controller.UpdateScore(score);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Test_UpdateState()
        {
            _controller.StartGame();

            var result = _controller.UpdateState(GameState.NotStarted);

            Assert.IsType<OkObjectResult>(result);
        }
    }

    public class WordRecall_IntegrationTests
    {
        private readonly WordRecallController _controller = new();

        [Fact]
        public void Test_RandomGameFunctionality()
        {
            var result = _controller.StartGame();

            // Test initial values
            var okResult = Assert.IsType<OkObjectResult>(result);

            // Test tweaked values
            int desiredScore = new Random().Next(1, 20);

            for (int i = 0; i < desiredScore; i++)
                _controller.UpdateScore(1);

            result = _controller.UpdateHealth(2);
            okResult = Assert.IsType<OkObjectResult>(result);

            // Test end game
            result = _controller.UpdateHealth(0);
            okResult = Assert.IsType<OkObjectResult>(result);

            result = _controller.UpdateState(GameState.Finished);
            okResult = Assert.IsType<OkObjectResult>(result);
        }
    }
}