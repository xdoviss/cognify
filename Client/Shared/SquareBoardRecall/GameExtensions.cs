namespace cognify.Client.Shared.SquareBoardRecall
{
    public static class GameExtensions
    {
        public static void ResetGameState(this SquareBoardRecallGame game)
        {
            game.Score = 0;
            game.FirstFlippedCard = null;
            game.SecondFlippedCard = null;
            game.StatusMessage = "Game started. Memorize the cards!";
            game.CanFlip = false;
           
        }
        public static void UpdateStatusMessage(this SquareBoardRecallGame game, string message)
        {
            game.StatusMessage = message;
            
        }
        public static void ResetFlippedCards(this SquareBoardRecallGame game)
        {
            if (game.FirstFlippedCard != null && game.SecondFlippedCard != null)
            {
                game.FirstFlippedCard = null;
                game.SecondFlippedCard = null;
            }
        }
    }
}
