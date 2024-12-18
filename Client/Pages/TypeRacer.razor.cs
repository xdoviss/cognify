using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using cognify.Shared;
using System.Linq;




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


        private string[] TargetWords;
        private int CurrentWordIndex = 0;
        private bool IsMistakeMadeInCurrentWord = false;
        private string CurrentWord => CurrentWordIndex < TargetWords.Length ? TargetWords[CurrentWordIndex] : "";
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
            CurrentWordIndex = 0;
            IsMistakeMadeInCurrentWord = false;

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
                    //var processor = new TextProcessor<TextMetadata>();
                    //TextMetadata metadata = processor.ProcessText(
                    //    text => new TextMetadata { OriginalText = text, WordCount = text.Split(' ').Length },
                    //    TargetText
                    //);
                    // Currently not needed code above (but in the future if we needed to bring it back)
                    TargetWords = TargetText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    WordCount = TargetWords.Length;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error loading text: {ex.Message}");
                TargetText = "Error fetching text. Please try again.";
                WordCount = 0;
            }
        }
        private async void HandleInput(ChangeEventArgs e)
        {
            string newInput = e.Value?.ToString() ?? "";

            if (newInput.EndsWith(" ")) // User pressed space
            {
                if (newInput.Trim() == CurrentWord) // Check if current word is correctly typed
                {
                    UserInput = ""; // Clear user input to start the next word
                    CurrentWordIndex++; // Move to next word
                    IsMistakeMadeInCurrentWord = false;

                    
                }
            }
            else
            {
                UserInput = newInput;
                CheckCurrentWord();
            }
            // Check if user finished typing all words
            if (CurrentWordIndex == TargetWords.Length -1 && UserInput.Trim() == CurrentWord)
            {
                await FinishGame();
                return;
                
            }

        }
        private void CheckCurrentWord()
        {
            if (CurrentWordIndex >= TargetWords.Length) return;

            string expectedWord = CurrentWord;
            if (!UserInput.Equals(expectedWord[..Math.Min(UserInput.Length, expectedWord.Length)], StringComparison.Ordinal))
            {
                if (!IsMistakeMadeInCurrentWord)
                {
                    MistakesCount++;
                    IsMistakeMadeInCurrentWord = true;
                }
            }
            HighlightCurrentProgress();
        }
        private void HighlightCurrentProgress()
        {
            HighlightedUserInput = "";

            for (int i = 0; i < TargetWords.Length; i++)
            {
                if (i < CurrentWordIndex)
                {
                    HighlightedUserInput += $"<span style='color:green;'>{TargetWords[i]} </span>";
                }
                else if (i == CurrentWordIndex)
                {
                    if (UserInput.Length > 0)
                    {
                        for (int j = 0; j < UserInput.Length; j++)
                        {
                            if (j < TargetWords[i].Length && UserInput[j] == TargetWords[i][j])
                            {
                                HighlightedUserInput += $"<span style='color:green;'>{UserInput[j]}</span>";
                            }
                            else
                            {
                                HighlightedUserInput += $"<span style='color:red;'>{UserInput[j]}</span>";
                            }
                        }

                        if (UserInput.Length < TargetWords[i].Length)
                        {
                            HighlightedUserInput += $"<span style='color:gray;'>{TargetWords[i][UserInput.Length..]}</span>";
                        }
                    }
                    else
                    {
                        HighlightedUserInput += TargetWords[i];
                    }
                    HighlightedUserInput += " ";
                }
                else
                {
                    HighlightedUserInput += $"{TargetWords[i]} ";
                }
            }
        }

        private async Task FinishGame()
        {
            IsGameStarted = false;
            ElapsedTime = (DateTime.Now - StartTime).TotalSeconds;
            HighlightedUserInput = "";
            UserInput = "";
            GameStatus = $"You made {MistakesCount} mistake(s) and finished in {ElapsedTime} seconds.";

            double totalWords = UserInput.Length / 5.0;
            double WPM = Math.Round(totalWords / (ElapsedTime / 60.0), 2);
            var gameResult = new GameResult(GameType.TypeRacer, WPM);
            System.Console.WriteLine(WPM);
            await Http.PostAsJsonAsync("api/LeaderBoard/results", gameResult);

            // Notifying server that game has finished
            await httpClient.PostAsJsonAsync("api/TypeRacer/finishGame", UserId);
        }
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        private void NavigateHome()
        {
            NavigationManager.NavigateTo("/");
        }

    }
}
