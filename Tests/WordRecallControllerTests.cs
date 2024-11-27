using cognify.Server.Controllers;
using Microsoft.AspNetCore.Mvc;

using cognify.Shared;

namespace cognify.Server.Tests;

public class WordRecallControllerTests
{
    private readonly WordRecallController m_controller = new();

    // Arrange → Act → Assert

    [Fact]
    public async Task Test_ReturnsNewWord()
    {
        var result = await m_controller.Get();

        var actionResult = Assert.IsType<ActionResult<string>>(result);
        Assert.IsType<OkObjectResult>(actionResult);
        Assert.IsType<string>(actionResult.Value);
    }

    [Fact]
    public void Test_StartGame()
    {
        var result = m_controller.StartGame();

        var actionResult = Assert.IsType<ActionResult>(result);
        Assert.IsType<OkObjectResult>(actionResult);
    }

    [Fact]
    public void Test_UpdateScore()
    {
        m_controller.StartGame();
        int score = new Random().Next(0, 20);

        var result = m_controller.UpdateScore(score);

        var actionResult = Assert.IsType<ActionResult>(result);
        Assert.IsType<OkObjectResult>(actionResult);
    }

    [Fact]
    public void Test_IsGameFinished()
    {
        m_controller.StartGame();

        var result = m_controller.IsGameFinished();

        Assert.IsType<ActionResult<bool>>(result);
        Assert.Equivalent(false, result.Value);
        Assert.IsType<OkObjectResult>(result.Result);

        result = m_controller.UpdateState(GameState.Finished);

        Assert.IsType<ActionResult<bool>>(result);
        Assert.True(result.Value);
        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public void Test_UpdateState()
    {
        m_controller.StartGame();

        var result = m_controller.UpdateState(GameState.NotStarted);

        var actionResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<WordRecallStatistics>(actionResult.Value);
        Assert.Equal(GameState.NotStarted, returnValue.State);
    }

    [Fact]
    public void Test_GameFunctionality()
    {
        var result = m_controller.StartGame();

        // Test initial values
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<WordRecallStatistics>(okResult.Value);

        Assert.Equal(GameState.InProgress, returnValue.State);
        Assert.Equal(0, returnValue.Score);
        Assert.Equal(3, returnValue.Health);

        // Test tweaked values
        m_controller.UpdateScore(returnValue.Score + 1);
        m_controller.UpdateScore(returnValue.Score + 3);

        result = m_controller.UpdateHealth(returnValue.Health - 1);
        okResult = Assert.IsType<OkObjectResult>(result);
        returnValue = Assert.IsType<WordRecallStatistics>(okResult.Value);

        Assert.Equal(GameState.InProgress, returnValue.State);
        Assert.Equal(4, returnValue.Score);
        Assert.Equal(2, returnValue.Health);

        // Test end game
        result = m_controller.UpdateHealth(0);
        okResult = Assert.IsType<OkObjectResult>(result);
        returnValue = Assert.IsType<WordRecallStatistics>(okResult.Value);

        Assert.Equal(GameState.Finished, returnValue.State);
        Assert.Equal(4, returnValue.Score);
        Assert.Equal(0, returnValue.Health);
    }
}