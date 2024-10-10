namespace cognify.Shared
{
    public enum GameState
    {
        NotStarted,
        InProgress,
        Finished,
    }
    public class WordRecallStatistics
    {
        public int Score { get; set; }
        public int Health { get; set; }
        public GameState State { get; set; }

        public WordRecallStatistics(GameState state)
        {
            Score = 0;
            Health = 3;
            State = state;
        }

        public bool IsGameFinished()
        {
            if (Health <= 0)
            {
                State = GameState.Finished;
                return true;
            }

            return false;
        }

        public int CompareTo(int other) => Score.CompareTo(other);
    }
}
