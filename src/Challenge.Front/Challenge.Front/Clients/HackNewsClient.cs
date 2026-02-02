using Challenge.Front.ViewModels;
namespace Challenge.Front.Clients;
public class HackNewsClient(HttpClient _httpClient)
{
    public async Task<HackNewsViewModel[]> GetAllHackNewsAsync()
        => await _httpClient.GetFromJsonAsync<HackNewsViewModel[]>("news/") ?? new HackNewsViewModel[0];

    public async Task<HackNewsViewModel[]> GetHackNewsTakeAsync(int number)
        => await _httpClient.GetFromJsonAsync<HackNewsViewModel[]>($"news/first/{number}") ?? new HackNewsViewModel[0];
}
