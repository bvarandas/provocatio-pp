using Challenge.Domain.Interfaces;
using Challenge.Infra.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Challenge.Application.Services;

public class InitialLoadHackNewsWorker : BackgroundService
{
    private readonly ILogger<InitialLoadHackNewsWorker> _logger;
    private readonly HackNewsClient _client;
    private readonly INewsCache _newsCache;
    public InitialLoadHackNewsWorker(ILogger<InitialLoadHackNewsWorker> logger, INewsCache newsCache, HackNewsClient client)
    {
        _client = client;
        _newsCache = newsCache;
        _logger = logger;
    }

    public async override Task StartAsync(CancellationToken stoppingToken)
    {
        await InitializeLoadAsync().ConfigureAwait(true);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }

    private async Task InitializeLoadAsync()
    {
        try
        {
            var listHas = await _newsCache.GetAllBestStoriesAsync();

            var list = await _client.GetBestStoriesAsync();

            var listToUpdate = list.Except(listHas.Value);

            await _newsCache.UpsertBestStoriesAsync(list);

            foreach (var id in listToUpdate)
            {
                try
                {
                    Task.Run(async () =>
                    {
                        await UpsertHackNewsAsync(id.ToString());
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }

    private async Task<bool> UpsertHackNewsAsync(string id)
    {
        var store = await _client.GetStoryByIDAsync(id);
        var result = await _newsCache.UpsertNewsAsync(store);

        return result.IsSuccess;
    }
}
