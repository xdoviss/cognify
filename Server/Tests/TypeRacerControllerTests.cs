using cognify.Shared;
using Newtonsoft.Json;
using System.Text;

using Xunit;

namespace cognify.Server.Tests
{
    public class TypeRacer_UnitTests
    {
        private readonly HttpClient _httpClient = new();
        private const string url = "http://localhost:5296/api/";

        [Fact]
        public async Task Test_GetRequest()
        {
            var result = await _httpClient.GetStringAsync(url + "TypeRacer/");
            
            Assert.IsType<string>(result);
        }

        [Fact]
        public async Task Test_StartAndStopGame()
        {
            var guid = new Guid().ToString();
            var content = new StringContent(JsonConvert.SerializeObject(guid), Encoding.UTF8, "application/json");

            int oldPlayerCount = await _httpClient.GetFromJsonAsync<int>(url + "Game/activePlayerCount");

            var response = await _httpClient.PostAsync(url + "TypeRacer/startGame", content);

            Assert.True(response.IsSuccessStatusCode);

            int newPlayerCount = await _httpClient.GetFromJsonAsync<int>(url + "Game/activePlayerCount");

            Assert.Equal(oldPlayerCount + 1, newPlayerCount);
            oldPlayerCount = newPlayerCount;

            response = await _httpClient.PostAsync(url + "TypeRacer/finishGame", content);

            Assert.True(response.IsSuccessStatusCode);

            newPlayerCount = await _httpClient.GetFromJsonAsync<int>(url + "Game/activePlayerCount");

            Assert.Equal(oldPlayerCount - 1, newPlayerCount);
        }
    }

    public class TypeRacer_IntegrationTests
    {
        private readonly HttpClient _httpClient = new();
        private const string url = "http://localhost:5296/api/";

        [Fact]
        public async Task Test_StartAndFinishGame()
        {
            // 1. Start random number of games
            int numberOfPlayers = new Random().Next(1, 5);
            var guids = new List<string>();

            for (int i = 0; i < numberOfPlayers; i++) {
                guids.Add(new Guid().ToString());
            }

            foreach (var content in guids.Select(guid => new StringContent(JsonConvert.SerializeObject(guid), Encoding.UTF8, "application/json")))
            {
                var response = await _httpClient.PostAsync(url + "TypeRacer/startGame", content);
                Assert.True(response.IsSuccessStatusCode);
            }

            int playerCount = await _httpClient.GetFromJsonAsync<int>(url + "Game/activePlayerCount");

            // 2. Get random number of words
            int numberOfRequests = new Random().Next(0, 6);

            while (numberOfRequests > 0)
            {
                var result = await _httpClient.GetStringAsync(url + "TypeRacer/");
            
                Assert.IsType<string>(result);

                numberOfRequests--;
            }

            // 3. Finish games
            foreach (var content in guids.Select(guid => new StringContent(JsonConvert.SerializeObject(guid), Encoding.UTF8, "application/json")))
            {
                var response = await _httpClient.PostAsync(url + "TypeRacer/finishGame", content);
                Assert.True(response.IsSuccessStatusCode);
            }
        }
    }
}
