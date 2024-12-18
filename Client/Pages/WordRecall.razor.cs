using System.Net.Http;
using System.Net.Http.Json;
using System.Security.AccessControl;

using cognify.Shared;
using Microsoft.AspNetCore.Components;

namespace cognify.Client.Pages
{
    public partial class WordRecall : ComponentBase
    {
        private string UserId = Guid.NewGuid().ToString(); 

        private Random random = new();
        private bool isHowToPlayVisible = false;

        private string currentWord;
        private List<string> seenWords = new();
        private int highscore = 0;

        WordRecallStatistics? GameStatistics = null;

        [Inject] private HttpClient Http { get; set; }

        private async Task StartGame()
        {
            var response = await Http.PostAsync("api/WordRecall/start", null);
            if (response.IsSuccessStatusCode)
            {
                await Http.PostAsJsonAsync("api/TypeRacer/startGame", UserId);
                GameStatistics = await response.Content.ReadFromJsonAsync<WordRecallStatistics>();
                await GetNewWord();
            }
            else
            {
                Console.WriteLine("Failed to start the game.");
            }
        }

        private void HideHowToPlay() => isHowToPlayVisible = false;
        private void ShowHowToPlay() => isHowToPlayVisible = true;

        protected override async Task OnInitializedAsync() => await GetNewWord();

        private async Task GetNewWord()
        {
            try
            {
                currentWord = await Http.GetStringAsync("api/WordRecall/");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching new word: {ex.Message}");
                currentWord = "Error fetching word";
            }
        }

        private async Task ShowWord()
        {
            if (random.Next(2) == 0 && seenWords.Count > 0)
            {
                currentWord = seenWords[random.Next(seenWords.Count)];
            }
            else
            {
                if (!seenWords.Contains(currentWord))
                {
                    seenWords.Add(currentWord);
                }

                await GetNewWord();
            }
        }

        private async Task CheckWord(bool isNew)
        {
            if ((isNew && !seenWords.Contains(currentWord)) || (!isNew && seenWords.Contains(currentWord)))
            {
                var response = await Http.PostAsJsonAsync("api/WordRecall/update-score", 1);
                if (response.IsSuccessStatusCode)
                {
                    GameStatistics = await response.Content.ReadFromJsonAsync<WordRecallStatistics>();
                }
            }
            else
            {
                var response = await Http.PostAsJsonAsync("api/WordRecall/update-health", GameStatistics.Health - 1);
                if (response.IsSuccessStatusCode)
                {
                    GameStatistics = await response.Content.ReadFromJsonAsync<WordRecallStatistics>();
                }
            }

            if (GameStatistics.Health > 0)
            {
                await ShowWord();
            }
            else
            {
                await Http.PostAsJsonAsync("api/TypeRacer/finishGame", UserId);

                var response = await Http.PostAsJsonAsync("api/WordRecall/update-state", GameState.Finished);
                if (response.IsSuccessStatusCode)
                    GameStatistics = await response.Content.ReadFromJsonAsync<WordRecallStatistics>();

                await PostGameResult();
            }
        }

        private async Task PostGameResult()
        {
            var gameResult = new GameResult(GameType.WordRecall, GameStatistics.Score, "Player1"); 

            await Http.PostAsJsonAsync("/api/LeaderBoard/results", gameResult);

            var allResults = await Http.GetFromJsonAsync<List<GameResult>>("api/LeaderBoard/results");
            var wordRecallResults = allResults.Where(r => r.GameType == GameType.WordRecall).ToList();

            highscore = (int)wordRecallResults.Max(gr => gr.Score);
        }
        [Inject] private NavigationManager Navigation { get; set; } // Add NavigationManager

        private void NavigateHome()
        {
            Navigation.NavigateTo("/"); // Navigate to the home page
        }
    }
}
