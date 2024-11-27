using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using cognify.Shared;



namespace cognify.Client.Pages
{
    public partial class TypeRacer
    {
        private string TargetText = "...";
        private string UserInput = "";
        private bool IsGameStarted = false;
        private DateTime StartTime;
        private double ElapsedTime = 0;
        private string GameStatus = "Click 'Start Game' to begin.";
        private int MistakesCount = 0;
        private string HighlightedUserInput = "";
        private int WordCount = 0;
        // Generating unique ID for the user
        private string UserId = Guid.NewGuid().ToString(); 

        [Inject]
        private HttpClient httpClient { get; set; }

        private async Task StartGame()
        {
            // Fetching the text from the local file
            await LoadTargetText(); 

            IsGameStarted = true;
            UserInput = "";
            ElapsedTime = 0;
            GameStatus = "Keep typing...";
            StartTime = DateTime.Now;
            MistakesCount = 0;
            HighlightedUserInput = "";

            // Notifying server that game has started
            await httpClient.PostAsJsonAsync("api/TypeRacer/startGame", UserId);


        }
        private async Task LoadTargetText()
        {
            try
            {
                // Calling the API endpoint to get the random text from the server side
                TargetText = await httpClient.GetStringAsync("api/TypeRacer");
                if (string.IsNullOrWhiteSpace(TargetText))
                {
                    TargetText = "Error fetching text. Please try again.";
                    WordCount = 0;
                }
                else
                {
                    var processor = new TextProcessor<TextMetadata>();
                    TextMetadata metadata = processor.ProcessText(
                        text => new TextMetadata { OriginalText = text, WordCount = text.Split(' ').Length },
                        TargetText
                    );
                    // Processing went good, so:
                    // Storing processed text and word count into the variables
                    TargetText = metadata.OriginalText;
                    WordCount = metadata.WordCount;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error loading text: {ex.Message}");
                TargetText = "Error fetching text. Please try again.";
                WordCount = 0;
            }
        }
        private void HandleInput(ChangeEventArgs e)
        {
            UserInput = e.Value?.ToString() ?? "";
            CheckUserInput(); // Call the check method after updating the UserInput
        }

        private async void CheckUserInput()
        {
            if (UserInput.Equals(TargetText, StringComparison.OrdinalIgnoreCase))
            {
                await FinishGame(); // Call FinishGame if the input matches the target text
            }
        }

        private async Task FinishGame()
        {
            IsGameStarted = false;
            ElapsedTime = (DateTime.Now - StartTime).TotalSeconds;

            MistakesCount = 0;
            HighlightedUserInput = "";

            for (int i = 0; i < Math.Max(UserInput.Length, TargetText.Length); i++)
            {
                if (i >= TargetText.Length)
                {
                    HighlightedUserInput += $"<span style='color:red;'>{UserInput[i]}</span>";
                    MistakesCount++;
                }
                else if (i >= UserInput.Length)
                {
                    HighlightedUserInput += $"<span style='color:red;'>_</span>";
                    MistakesCount++;
                }
                else if (UserInput[i] != TargetText[i])
                {
                    HighlightedUserInput += $"<span style='color:red;'>{UserInput[i]}</span>";
                    MistakesCount++;
                }
                else
                {
                    HighlightedUserInput += UserInput[i];
                }
            }

            GameStatus = $"You made {MistakesCount} mistake(s) and finished in {ElapsedTime} seconds.";

            double totalWords = UserInput.Length / 5.0;
            double WPM = Math.Round(totalWords / (ElapsedTime / 60.0), 2);
            var gameResult = new GameResult(GameType.TypeRacer, WPM);
            System.Console.WriteLine(WPM);
            await Http.PostAsJsonAsync("api/LeaderBoard/results", gameResult);

            // Notifying server that game has finished
            await httpClient.PostAsJsonAsync("api/TypeRacer/finishGame", UserId);
        }
    }
}
