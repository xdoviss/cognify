using Microsoft.AspNetCore.Mvc;

namespace cognify.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WordRecallController : ControllerBase
{
    private static readonly List<string> SeenWords = new();
    private static readonly HttpClient HttpClient = new();

    [HttpGet]
    public async Task<ActionResult<string>> Get()
    {
        string word = await GetNewWord();

        while (SeenWords.Contains(word))
            word = await GetNewWord();

        SeenWords.Add(word);

        return word;
    }

    private static async Task<string> GetNewWord()
    {
        var response = await HttpClient.GetStringAsync("https://random-word-api.herokuapp.com/word");
        var words = System.Text.Json.JsonSerializer.Deserialize<List<string>>(response);
        var word = words[0];

        return word;
    }
}