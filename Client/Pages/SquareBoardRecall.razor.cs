using System.Net.Http.Json;
using cognify.Client.Shared.SquareBoardRecall;

namespace cognify.Client.Pages
{
    public partial class SquareBoardRecall
    {
        private SquareBoardRecallGame game = new SquareBoardRecallGame();

        private async Task StartGame()
        {
            game.IsGameStarted = true;
            game.ResetGameState();

            // Use named argument for difficultyLevel, memorizationTime will use the default
            await InitializeGame(difficultyLevel: game.difficultyLevel);

            await FlipAllCards(true);
            game.UpdateStatusMessage("Memorize the cards!");
            game.CanFlip = false;
            StateHasChanged();

            // Adjust memorization time based on difficulty level, using the default value of 5000 if not provided
            int memorizationTime = 5000 - (game.difficultyLevel * 500);
            if (memorizationTime < 500)
            {
                memorizationTime = 500;
            }
            await Task.Delay(memorizationTime);

            await FlipAllCards(false);

            game.UpdateStatusMessage("Start matching the cards!");
            game.CanFlip = true;
            StateHasChanged();
        }

        private void FlipCard(SquareBoardRecallGame.Card card)
        {
            if (!game.CanFlip || card.IsFlipped)
                return;

            card.IsFlipped = true;

            if (game.FirstFlippedCard == null)
            {
                game.FirstFlippedCard = card;
            }
            else if (game.SecondFlippedCard == null)
            {
                game.SecondFlippedCard = card;
                CheckForMatch();
            }
        }

        private async void CheckForMatch()
        {
            if (game.FirstFlippedCard != null && game.SecondFlippedCard != null)
            {
                // Compare the cards using the Equals method
                if (game.FirstFlippedCard.Id == game.SecondFlippedCard.Id)
                {
                    game.Score++;
                    game.UpdateStatusMessage("It's a match!");
                    game.ResetFlippedCards();
                    StateHasChanged();

                    // Check if all cards are flipped
                    if (game.Cards.All(c => c.IsFlipped))
                    {
                        game.UpdateStatusMessage("All matched! Starting new round...");
                        await Task.Delay(2000);
                        await StartNewRound();
                    }
                }
                else
                {
                    game.CanFlip = false;
                    game.Health--;
                    game.UpdateStatusMessage("No match!");

                    if (game.Health <= 0)
                    {
                        game.UpdateStatusMessage("Game Over!");
                        EndGame();
                        return;
                    }

                    await Task.Delay(1000);

                    game.FirstFlippedCard.IsFlipped = false;
                    game.SecondFlippedCard.IsFlipped = false;

                    game.ResetFlippedCards();
                    game.CanFlip = true;
                    StateHasChanged();
                }
            }
        }

        private async Task InitializeGame(int difficultyLevel = 1, int memorizationTime = 5000)
        {
            // Adjust the memorization time based on the difficulty level
            memorizationTime -= (difficultyLevel * 500);
            if (memorizationTime < 500)
            {
                memorizationTime = 500;
            }

            try
            {
                // Fetch the cards from the API (keeping the number of cards fixed for now)
                game.Cards = await Http.GetFromJsonAsync<List<SquareBoardRecallGame.Card>>("/api/SquareBoardRecall/initialize-game");

                // Update game message
                game.UpdateStatusMessage($"Level {difficultyLevel}: Memorize the cards!");

                // Flip all cards to show them
                await FlipAllCards(true);

                // Wait for the adjusted memorization time
                await Task.Delay(memorizationTime);

                // Flip cards back
                await FlipAllCards(false);

                // Ready for matching
                game.CanFlip = true;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                game.UpdateStatusMessage("Error initializing game: " + ex.Message);
            }
        }

        private async Task StartNewRound()
        {
            game.difficultyLevel++; // Increase difficulty with each new round

            // Use named arguments for both difficultyLevel and memorizationTime
            await InitializeGame(difficultyLevel: game.difficultyLevel, memorizationTime: 5000);

            await FlipAllCards(true);
            game.UpdateStatusMessage($"Level {game.difficultyLevel}: Memorize the cards!");
            StateHasChanged();
            game.CanFlip = false;

            // Adjust memorization time based on difficulty level
            int memorizationTime = 5000 - (game.difficultyLevel * 500);
            if (memorizationTime < 500)
            {
                memorizationTime = 500;
            }
            await Task.Delay(memorizationTime);

            await FlipAllCards(false);

            game.UpdateStatusMessage("New round started! Match the cards!");
            game.CanFlip = true;
            StateHasChanged();
        }

        private void EndGame()
        {
            game.IsGameStarted = false;
            game.UpdateStatusMessage("Game Over! Click 'Start' to try again.");
        }

        private async Task FlipAllCards(bool flip)
        {
            foreach (var card in game.Cards)
            {
                card.IsFlipped = flip;
            }
            await InvokeAsync(StateHasChanged);
        }
    }
}
