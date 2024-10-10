namespace cognify.Client.Shared.SquareBoardRecall
{
    public class SquareBoardRecallGame
    {
        public bool IsGameStarted { get; set; } = false;
        public List<Card> Cards { get; set; } = new List<Card>();
        public Card? FirstFlippedCard { get; set; }
        public Card? SecondFlippedCard { get; set; }
        public string StatusMessage { get; set; } = "Click 'Start' to begin the game.";
        public int Score { get; set; } = 0;
        public int Health { get; set; } = 3;
        public bool CanFlip { get; set; } = false;
        public class Card
        {
            public int Id { get; set; }
            public string? Image { get; set; }
            public bool IsFlipped { get; set; }
        }
    }
}
