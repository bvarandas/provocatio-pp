using Challenge.Application.Dto;
using Challenge.Application.Interfaces;
using Challenge.Domain.Interfaces;
using Challenge.Domain.Models;
using Challenge.Domain.Models.Results;
using Challenge.Infra.Client;
using Microsoft.Extensions.Logging;

namespace Challenge.Application.Services;

public class NewsService : INewsService
{
    private readonly INewsCache _newsCache;
    private readonly ILogger<NewsService> _logger;
    private readonly HackNewsClient _client;
    public NewsService(ILogger<NewsService> logger, INewsCache newsCache, HackNewsClient client)
    {
        _logger = logger;
        _newsCache = newsCache;
        _client = client;

        //InitializeFirebase();
    }

    public async Task<Result<List<int>>> GetBestStoriesAsync()
    {
        var result = new List<int>();

        try
        {
            result = await _client.GetBestStoriesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        return Result<List<int>>.Ok(result);
    }

    //CacheAside
    public async Task<Result<List<HackNewsDto>>> GetNewsTakeNumber(int number)
    {
        var result = new List<News>();
        try
        {
            var listCache = await _newsCache.GetAllNewsAsync();

            if (listCache is null)
            {
                var list = await _client.GetBestStoriesAsync();

                foreach (var id in list)
                {
                    var news = await _client.GetStoryByIDAsync(id.ToString());

                    await _newsCache.UpsertNewsAsync(news);

                    result.Add(news);
                }

            }

            result = listCache.Value
                .OrderByDescending(c => c.Score)
                .Take(number)
                .ToList();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        return Result<List<HackNewsDto>>.Ok(result.ToDto());
    }

    //CacheAside
    public async Task<Result<List<HackNewsDto>>> GetAllNewsAsync()
    {
        var result = new List<News>();
        try
        {
            var listCache = await _newsCache.GetAllNewsAsync();

            if (listCache is null)
            {
                var list = await _client.GetBestStoriesAsync();

                foreach (var id in list)
                {
                    var news = await _client.GetStoryByIDAsync(id.ToString());

                    await _newsCache.UpsertNewsAsync(news);
                }
            }

            result = listCache.Value.ToList();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        return Result<List<HackNewsDto>>.Ok(result.ToDto());
    }

    //Cache aside
    public async Task<Result<HackNewsDto>> GetNewsByIDAsync(int Id)
    {
        var result = new News();

        try
        {
            var cache = await _newsCache.GetNewsByIDAsync(Id);

            if (cache is null)
            {
                result = cache.Value;
            }

        }
        catch (Exception)
        {

            throw;
        }

        return Result<HackNewsDto>.Ok(result.ToDto());
    }
}
