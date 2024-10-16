using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using cognify.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cognify.Client.Pages
{
    public partial class LeaderBoard : ComponentBase
    {
        private string selectedGame = "TypeRacer";  // Default selected game
        public List<GameResult> AllGameResults { get; set; } = new List<GameResult>();
        public List<GameResult> FilteredResults { get; set; } = new List<GameResult>();

        [Inject]
        private HttpClient HttpClient { get; set; }

        protected override async Task OnInitializedAsync()
        {
            // Load game results from the server
            await LoadGameResultsAsync();

            // Filter results
            FilterResults();
        }

        private async Task LoadGameResultsAsync()
        {
            AllGameResults = await HttpClient.GetFromJsonAsync<List<GameResult>>("api/LeaderBoard/results");
        }

        private void FilterResults()
        {
            GameType gameType = selectedGame switch
            {
                "TypeRacer" => GameType.TypeRacer,
                "WordRecall" => GameType.WordRecall,
                "BoardRecall" => GameType.BoardRecall,
                _ => GameType.TypeRacer // Default fallback to TypeRacer
            };

            FilteredResults = AllGameResults.Where(r => r.GameType == gameType).ToList();
        }

        private void OnGameTypeChange(ChangeEventArgs e)
        {
            // Update selectedGame
            selectedGame = e.Value.ToString();
            // Re-filter results
            FilterResults();
        }
    }
}
