using cognify.Server.Controllers;
using Xunit;
using Microsoft.AspNetCore.Mvc;

namespace cognify.Server.Tests;

public class SquareBoard_UnitTests
{
    private readonly SquareBoardRecallController _controller = new();

    [Fact]
    public void Test_ReturnsDeck()
    {
        var result = _controller.InitializeGame();

        var actionResult = Assert.IsType<ActionResult<List<SquareBoardRecallController.Card>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var cards = Assert.IsType<List<SquareBoardRecallController.Card>>(okResult.Value);

        Assert.Equal(8, cards.Count);
        Assert.Equal(4, cards.Select(c => c.Id).Distinct().Count());
    }

    [Fact]
    public void Test_ShufflesDeck()
    {
        var result1 = _controller.InitializeGame();
        var result2 = _controller.InitializeGame();

        var actionResult1 = Assert.IsType<ActionResult<List<SquareBoardRecallController.Card>>>(result1);
        var okResult1 = Assert.IsType<OkObjectResult>(actionResult1.Result);
        var cards1 = Assert.IsType<List<SquareBoardRecallController.Card>>(okResult1.Value);

        var actionResult2 = Assert.IsType<ActionResult<List<SquareBoardRecallController.Card>>>(result2);
        var okResult2 = Assert.IsType<OkObjectResult>(actionResult2.Result);
        var cards2 = Assert.IsType<List<SquareBoardRecallController.Card>>(okResult2.Value);

        Assert.NotEqual(cards1.Select(c => c.Id), cards2.Select(c => c.Id));
    }
}

public class SquareBoard_IntegrationTest
{
    [Fact]
    public void Test_GameFunctionality()
    {
        var unitTests = new SquareBoard_UnitTests();

        unitTests.Test_ReturnsDeck();
        unitTests.Test_ShufflesDeck();
        unitTests.Test_ShufflesDeck();
        unitTests.Test_ShufflesDeck();
    }
}