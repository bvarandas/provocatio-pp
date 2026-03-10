using Challenge.Domain.Interfaces;
using Challenge.Domain.Models;
using Challenge.Domain.Models.Results;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Challenge.Infra.Cache;

public class NewsCache : INewsCache
{
    private readonly ILogger<NewsCache> _logger;
    private static ConcurrentDictionary<int, News> _dicHackNews = null;
    private static ConcurrentStack<int> _stackBestStories = null;
    private static ConcurrentQueue<int> _queueBestStories = null;

    public NewsCache(ILogger<NewsCache> logger)
    {
        _logger = logger;
        _dicHackNews = new ConcurrentDictionary<int, News>();
        _stackBestStories = new ConcurrentStack<int>();
        _queueBestStories = new ConcurrentQueue<int>();
    }

    public async Task<Result<IEnumerable<int>>> GetAllBestStoriesAsync()
    {
        var result = Enumerable.Empty<int>();
        result = _stackBestStories
            .ToList();
        return Result<IEnumerable<int>>.Ok(result);
    }

    public async Task<Result<IEnumerable<News>>> GetAllNewsAsync()
    {
        var result = Enumerable.Empty<News>();

        result = _dicHackNews.Values.ToList()
            .OrderByDescending(c => c.Score);

        return Result<IEnumerable<News>>.Ok(result);
    }

    public async Task<Result<News>> GetNewsByIDAsync(int id)
    {
        if (_dicHackNews.TryGetValue(id, out News hash))
        {
            return Result<News>.Ok(hash);
        }

        return Result<News>.Ok(hash);
    }

    public async Task<Result> RemoveNewsAsync(News news)
    {
        try
        {
            if (_dicHackNews.TryRemove(news.Id, out News newsOld))
            {
                _logger.LogInformation("Hacker News removed with success cache");
            }
            else
                _logger.LogInformation("Issue on removed Hacker news cache");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);

            return Result.Fail(ex.Message);
        }

        return Result.Ok();
    }

    public async Task<Result> UpsertBestStoriesAsync(List<int> litBestStories)
    {
        litBestStories.ForEach(story =>
        {
            _stackBestStories.Push(story);
            _queueBestStories.Enqueue(story);
        });

        return Result.Ok();
    }

    public async Task<Result> UpsertNewsAsync(News news)
    {
        try
        {
            _dicHackNews.AddOrUpdate(news.Id, news, (key, oldValue) => news);

            _logger.LogInformation("News upserted with success cache");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);

            return Result.Fail(ex.Message);
        }

        return Result.Ok();
    }
}