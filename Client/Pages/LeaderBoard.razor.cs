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

        // Filtered lists for each game type
        public List<GameResult> TypeRacerResults { get; set; } = new List<GameResult>();
        public List<GameResult> WordRecallResults { get; set; } = new List<GameResult>();
        public List<GameResult> BoardRecallResults { get; set; } = new List<GameResult>();
        [Inject]
        private HttpClient HttpClient { get; set; }

        protected override async Task OnInitializedAsync()
        {
            // Load game results from the GameResultService
            await LoadGameResultsAsync();

        }

        private async Task LoadGameResultsAsync()
        {
            var allGameResults = await HttpClient.GetFromJsonAsync<List<GameResult>>("api/LeaderBoard/results");

            if (allGameResults != null)
            {
                foreach (var result in allGameResults)
                {
                    if (result.TypeRacerWPM > 0)
                        TypeRacerResults.Add(result);
                    if (result.WordRecallHS > 0)
                        WordRecallResults.Add(result);
                    if (result.BoardRecallHS > 0)
                        BoardRecallResults.Add(result);
                }
            }
        }
    }
}
