using Challenge.Domain.Models;
using System.Net.Http.Json;
namespace Challenge.Infra.Client;
public class HackNewsClient(HttpClient _httpClient)
{
    public HttpClient HttpClient => _httpClient;
    public async Task<List<int>> GetBestStoriesAsync()
        => await _httpClient.GetFromJsonAsync<List<int>>("v0/beststories.json") ?? new List<int>();
    public async Task<News> GetStoryByIDAsync(string id)
        => await _httpClient.GetFromJsonAsync<News>($"v0/item/{id}.json") ?? throw new Exception("Could not find Store!");
    public async Task<News> GetStoryMaxItemAsync()
        => await _httpClient.GetFromJsonAsync<News>($"v0/maxitem.json") ?? throw new Exception("Could not max item find Store!");
    public string GetUrlMaxItem() => $"{_httpClient.BaseAddress}v0/maxitem.json";
}