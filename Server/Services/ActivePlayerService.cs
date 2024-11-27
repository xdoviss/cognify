using System.Collections.Concurrent;

public class ActivePlayerService
{
    private readonly ConcurrentDictionary<string, string> _activePlayers = new();

    public void AddPlayer(string userId)
    {
        _activePlayers.TryAdd(userId, userId);
    }

    public void RemovePlayer(string userId)
    {
        _activePlayers.TryRemove(userId, out _);
    }

    public int GetActivePlayerCount()
    {
        return _activePlayers.Count;
    }

    public IEnumerable<string> GetAllActivePlayers()
    {
        return _activePlayers.Values;
    }
}
