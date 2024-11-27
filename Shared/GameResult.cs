namespace cognify.Shared
{
    public class GameResult
    {
        public int Id { get; set; } 
        public GameType GameType { get; set; }  // Enum to specify which game we register the score to
        public double Score { get; set; }       // Common score property for all games
        public string UserName { get; set; }

        // Constructor to set game type and score
        public GameResult(GameType gameType, double score, string userName = "Default user")
        {
            GameType = gameType;
            Score = score;
            UserName = userName;
        }
    }
}
