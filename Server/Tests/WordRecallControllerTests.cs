﻿using cognify.Server.Controllers;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using cognify.Shared;

namespace cognify.Server.Tests;

public class WordRecallControllerTests
{
    private readonly WordRecallController _controller = new();

    // Arrange → Act → Assert

    [Fact]
    public async Task Test_ReturnsNewWord()
    {
        var result = await _controller.Get();

        var actionResult = Assert.IsType<ActionResult<string>>(result);
        
        Assert.IsType<string>(actionResult.Value);
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

        var actionResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<WordRecallStatistics>(actionResult.Value);
        Assert.Equal(score, returnValue.Score);
    }

    [Fact]
    public void Test_UpdateState()
    {
        _controller.StartGame();

        var result = _controller.UpdateState(GameState.NotStarted);

        var actionResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<WordRecallStatistics>(actionResult.Value);
        Assert.Equal(GameState.NotStarted, returnValue.State);
    }

    [Fact]
    public void Test_GameFunctionality()
    {
        var result = _controller.StartGame();

        // Test initial values
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<WordRecallStatistics>(okResult.Value);

        Assert.Equal(GameState.InProgress, returnValue.State);
        Assert.Equal(0, returnValue.Score);
        Assert.Equal(3, returnValue.Health);

        // Test tweaked values
        _controller.UpdateScore(returnValue.Score + 1);
        _controller.UpdateScore(returnValue.Score + 3);

        result = _controller.UpdateHealth(returnValue.Health - 1);
        okResult = Assert.IsType<OkObjectResult>(result);
        returnValue = Assert.IsType<WordRecallStatistics>(okResult.Value);

        Assert.Equal(GameState.InProgress, returnValue.State);
        Assert.Equal(5, returnValue.Score);
        Assert.Equal(2, returnValue.Health);

        // Test end game
        result = _controller.UpdateHealth(0);
        okResult = Assert.IsType<OkObjectResult>(result);
        returnValue = Assert.IsType<WordRecallStatistics>(okResult.Value);

        Assert.Equal(0, returnValue.Health);

        var isFinishedResult = _controller.IsGameFinished();
        var isFinishedOkResult = Assert.IsType<ActionResult<bool>>(isFinishedResult);
        Assert.IsType<bool>(isFinishedOkResult.Value);
    }
}