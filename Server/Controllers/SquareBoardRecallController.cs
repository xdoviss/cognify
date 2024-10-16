using Microsoft.AspNetCore.Mvc;


namespace cognify.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SquareBoardRecallController : ControllerBase
{
    [HttpGet("initialize-game")]
    public ActionResult<List<Card>> InitializeGame()
    {
        // Create a shuffled deck of card pairs (4 pairs for an 8-card game)
        var cards = GenerateShuffledDeck(8);
        return Ok(cards);
    }

    private List<Card> GenerateShuffledDeck(int numberOfCards)
    {
        var cards = new List<Card>();

        // Create pairs of cards with unique Ids and image paths
        for (int i = 1; i <= numberOfCards / 2; i++)
        {
            var card1 = new Card(i, $"/images/card{i}.png");
            var card2 = new Card(i, $"/images/card{i}.png");

            cards.Add(card1);
            cards.Add(card2);
        }

        // Shuffle the cards
        var rng = new Random();
        cards = cards.OrderBy(x => rng.Next()).ToList();

        return cards;
    }

    public record Card(int Id, string? Image)
    {
    }
}
