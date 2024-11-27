using System.Net.Http;
using System.Net.Http.Json;
using cognify.Client.Shared.SquareBoardRecall;
using Microsoft.AspNetCore.Components;
using cognify.Shared;

namespace cognify.Client.Pages
{
    public partial class SquareBoardRecall
    {
        private SquareBoardRecallGame game = new SquareBoardRecallGame();
        private GameDifficulty selectedDifficulty = GameDifficulty.Easy;
        private string UserId = Guid.NewGuid().ToString();
        [Inject] private HttpClient HttpClientInstance { get; set; }
        private void OnDifficultyChange(ChangeEventArgs e)
        {
            if (Enum.TryParse<GameDifficulty>(e.Value.ToString(), out var difficulty))
            {
                selectedDifficulty = difficulty;
            }
        }

        private async Task StartGame()
        {
            await HttpClientInstance.PostAsJsonAsync("api/TypeRacer/startGame", UserId);
            game.IsGameStarted = true;
            game.ResetGameState();
            switch (selectedDifficulty)
            {
                case GameDifficulty.Easy:
                    game.Health = 5;
                    break;
                case GameDifficulty.Medium:
                    game.Health = 3;
                    break;
                case GameDifficulty.Hard:
                    game.Health = 1;
                    break;
            }
            await InitializeGame(difficultyLevel: game.difficultyLevel);
            await FlipAllCards(true);
            game.UpdateStatusMessage("Memorize the cards!");
            game.CanFlip = false;
            StateHasChanged();
            int memorizationTime = 3000;
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
                        await EndGame();
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

        private async Task InitializeGame(int difficultyLevel = 1, GameDifficulty difficulty = GameDifficulty.Easy, int memorizationTime = 5000)
        {
            switch (difficulty)
            {
                case GameDifficulty.Easy:
                    memorizationTime = 3000;
                    break;
                case GameDifficulty.Medium:
                    memorizationTime = 2000;
                    break;
                case GameDifficulty.Hard:
                    memorizationTime = 1000;
                    break;
            }

            memorizationTime -= (difficultyLevel * 500);
            if (memorizationTime < 500)
            {
                memorizationTime = 500;
            }

            try
            {
                game.Cards = await Http.GetFromJsonAsync<List<SquareBoardRecallGame.Card>>("/api/SquareBoardRecall/initialize-game");
                game.UpdateStatusMessage($"Level {difficultyLevel}: Memorize the cards!");
                await FlipAllCards(true);
                await Task.Delay(memorizationTime);
                await FlipAllCards(false);
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
            game.difficultyLevel++;
            
            await InitializeGame(difficultyLevel: game.difficultyLevel, memorizationTime: 5000);
            await FlipAllCards(true);
            game.UpdateStatusMessage($"Level {game.difficultyLevel}: Memorize the cards!");
            StateHasChanged();
            game.CanFlip = false;
            int memorizationTime = 5000;
            switch (selectedDifficulty)
            {
                case GameDifficulty.Easy:
                    memorizationTime = 3000;
                    break;
                case GameDifficulty.Medium:
                    memorizationTime = 2000;
                    break;
                case GameDifficulty.Hard:
                    memorizationTime = 1000;
                    break;
            }
            int change = memorizationTime - (game.difficultyLevel * 500);
            if (change < 500)
            {
                memorizationTime = 500;
            }
            await Task.Delay(memorizationTime);

            await FlipAllCards(false);

            game.UpdateStatusMessage("New round started! Match the cards!");
            game.CanFlip = true;
            StateHasChanged();
        }

        private async Task EndGame()
        {
            await HttpClientInstance.PostAsJsonAsync("api/TypeRacer/finishGame", UserId);
            game.IsGameStarted = false;
            game.UpdateStatusMessage("Game Over! Click 'Start' to try again.");
            await PostGameResult();
            game.ResetGameState();
        }
        private async Task PostGameResult()
        {
            var gameResult = new GameResult(GameType.BoardRecall, game.Score, "Player1"); 

            await Http.PostAsJsonAsync("/api/LeaderBoard/results", gameResult);
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
