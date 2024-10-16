using System.Collections.Generic;
using cognify.Shared;

public class GameResultService
{
    private List<GameResult> _gameResults = new List<GameResult>();

    public void AddResult(GameResult result)
    {
        _gameResults.Add(result);
    }

    public List<GameResult> GetResults()
    {
        return _gameResults;
    }
}
