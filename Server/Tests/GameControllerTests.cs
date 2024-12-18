using cognify.Server.Controllers;
using Microsoft.AspNetCore.Mvc;

using Xunit;

namespace cognify.Server.Tests
{
    public class GameController_UnitTests
    {
        private readonly GameController _controller;

        public GameController_UnitTests()
        {
            var activePlayerService = new ActivePlayerService();
            _controller = new GameController(activePlayerService);
        }

        [Fact]
        public void GetActivePlayerCount_ReturnsOkResult()
        {
            // Act
            var result = _controller.GetActivePlayerCount();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}