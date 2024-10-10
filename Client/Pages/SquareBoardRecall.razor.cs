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

            await InitializeGame();

            foreach (var card in game.Cards)
            {
                card.IsFlipped = true;
            }
            game.UpdateStatusMessage("Memorize the cards!");
            game.CanFlip = false;
            StateHasChanged();

            await Task.Delay(3000);

            foreach (var card in game.Cards)
            {
                card.IsFlipped = false;
            }

            game.UpdateStatusMessage("Start matching the cards!");
            game.CanFlip = true;
            StateHasChanged();
        }

        private void FlipCard(SquareBoardRecallGame.Card card)
        {
            if (!game.CanFlip || card.IsFlipped)
                return;
            {
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
        }

        private async void CheckForMatch()
        {
            if (game.FirstFlippedCard != null && game.SecondFlippedCard != null)
            {
                if (game.FirstFlippedCard.Id == game.SecondFlippedCard.Id)
                {
                    game.Score++;
                    game.UpdateStatusMessage("It's a match!");
                    game.ResetFlippedCards();
                    StateHasChanged();

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

        private async Task InitializeGame()
        {
            try
            {
                game.Cards = await Http.GetFromJsonAsync<List<SquareBoardRecallGame.Card>>("/api/SquareBoardRecall/initialize-game");
            }
            catch (Exception ex)
            {
                game.UpdateStatusMessage("Error initializing game: " + ex.Message);
            }
        }

        private async Task StartNewRound()
        {
            await InitializeGame();

            foreach (var card in game.Cards)
            {
                card.IsFlipped = true;
            }
            game.UpdateStatusMessage("Memorize the cards!");
            StateHasChanged();
            game.CanFlip = false;
            await Task.Delay(3000);

            foreach (var card in game.Cards)
            {
                card.IsFlipped = false;
            }

            game.UpdateStatusMessage("New round started! Match the cards!");
            game.CanFlip = true;
            StateHasChanged();
        }

        private void EndGame()
        {
            game.IsGameStarted = false;
            game.UpdateStatusMessage("Game Over! Click 'Start' to try again.");
        }
    }
}

