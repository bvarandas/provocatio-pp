using Challenge.Domain.Interfaces;
using Challenge.Domain.Models;
using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace Challenge.Infra.Cache;

public class NewsCache : INewsCache
{
    private readonly ILogger<NewsCache> _logger;
    private readonly ConnectionDragonflyDB _config;
    private readonly ConnectionMultiplexer _dragonfly;
    private readonly IDatabase _db;
    private readonly int _database;

    private RedisKey keystories = new RedisKey("stories");
    private RedisKey keynews = new RedisKey("news");

    public NewsCache(IOptions<ConnectionDragonflyDB> config, ILogger<NewsCache> logger)
    {
        _logger = logger;
        _config = config.Value;

        _dragonfly = ConnectionMultiplexer.Connect(_config.ConnectionString, options =>
        {
            options.ReconnectRetryPolicy = new ExponentialRetry(5000, 1000 * 60);
        });
        _database = 1;
        _db = _dragonfly.GetDatabase(_database);
    }


    public async Task<Result<IEnumerable<int>>> GetAllBestStoriesAsync()
    {
        var hash = new List<int>();

        var result = Enumerable.Empty<int>();

        var valueKey = new RedisValue("list");

        var hashEntry = await _db.HashGetAsync(keystories, valueKey);

        if (hashEntry.HasValue)
        {
            var value = JsonSerializer.Deserialize<List<int>>(hashEntry.ToString());
            hash.AddRange(value!);
        }

        result = hash;

        return Result.Ok(result);
    }

    public async Task<Result<IEnumerable<News>>> GetAllNewsAsync()
    {
        var hash = new List<News>();

        var result = Enumerable.Empty<News>();

        var hashEntry = await _db.HashGetAllAsync(keynews);

        foreach (var item in hashEntry)
        {
            var value = JsonSerializer.Deserialize<News>(item.Value!);
            hash.Add(value!);
        }

        result = hash;

        return Result.Ok(result);
    }

    public async Task<Result<News>> GetNewsByIDAsync(int id)
    {
        var hash = new News();

        var hashEntry = await _db.HashGetAsync(keynews, new RedisValue(id.ToString()));

        if (hashEntry.HasValue)
        {
            hash = JsonSerializer.Deserialize<News>(hashEntry.ToString());
        }

        return Result.Ok(hash);
    }

    public async Task<Result> RemoveNewsAsync(News news)
    {
        ITransaction transaction = _db.CreateTransaction();

        try
        {
            RedisValue value = new RedisValue(news.Id.ToString());

            _ = transaction.HashDeleteAsync(keynews, value);

            bool committed = await transaction.ExecuteAsync();

            _logger.LogInformation(committed ? "Hacker News removed with success cache" : "Issue on removed Hacker news cache");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);

            return Result.Fail(new Error(ex.Message));
        }

        return Result.Ok();
    }

    public async Task<Result> UpsertBestStoriesAsync(List<int> litBestStories)
    {
        RedisValue value = new RedisValue(JsonSerializer.Serialize(litBestStories));
        await _db.HashSetAsync(keystories, new HashEntry[] { new HashEntry(new RedisValue("list"), value) });
        return Result.Ok();
    }

    public async Task<Result> UpsertNewsAsync(News news)
    {
        try
        {
            ITransaction transaction = _db.CreateTransaction();

            RedisValue value = new RedisValue(JsonSerializer.Serialize(news));

            _ = transaction.HashSetAsync(keynews, new HashEntry[] { new HashEntry(new RedisValue(news.Id.ToString()), value) });

            bool committed = await transaction.ExecuteAsync();

            _logger.LogInformation(committed ? "News upserted with success cache" : "Issue on upsert news cache");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);

            return Result.Fail(new Error(ex.Message));
        }

        return Result.Ok();
    }
}
