namespace cognify.Client.Shared.SquareBoardRecall
{
    public static class GameExtensions
    {
        // Extension method to reset the game state
        public static void ResetGameState(this SquareBoardRecallGame game)
        {
            game.Score = 0;
            game.Health = 3;
            game.FirstFlippedCard = null;
            game.SecondFlippedCard = null;
            game.StatusMessage = "Game started. Memorize the cards!";
        }

        // Extension method to update the status message
        public static void UpdateStatusMessage(this SquareBoardRecallGame game, string message)
        {
            game.StatusMessage = message;
            
        }

        // Extension method to reset flipped cards
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
