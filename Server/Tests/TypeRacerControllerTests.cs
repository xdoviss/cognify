using cognify.Server.Controllers;
using Xunit;

namespace cognify.Server.Tests
{
    public class TypeRacerControllerTests
    {
        private readonly TypeRacerController _controller;

        public TypeRacerControllerTests()
        {
            var textLoaderService = new TextLoaderService();
            var gameResultService = new GameResultService();

            _controller = new TypeRacerController(textLoaderService, gameResultService);
        }

        [Fact]
        public async Task Test_GetRandomText()
        {
            var result = await _controller.GetRandomText();
            Assert.IsType(null, result); // temporary, need to rediscuss how to do testing for this
        }
    }
}
