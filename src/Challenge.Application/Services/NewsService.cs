using Challenge.Domain.Interfaces;
using Challenge.Domain.Models;
using Challenge.Infra.Client;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using FluentResults;
using Google.Apis.Auth.OAuth2;
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

    public static void InitializeFirebase()
    {
        string path = "path/to/your/serviceAccountKey.json";
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

        var app = FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromFile(path)
        });

        FirebaseMessaging.GetMessaging(app);




    }
    private async Task SendMessageToTopicAsync(string topicName, string title, string body)
    {
        var message = new Message()
        {
            Notification = new Notification()
            {
                Title = title,
                Body = body
            },
            Topic = topicName
        };

        string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
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

        return Result.Ok(result);
    }

    //CacheAside
    public async Task<Result<List<News>>> GetNewsTakeNumber(int number)
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
                .OrderBy(c => c.Time)
                .Take(number)
                .ToList();

        }
        catch (Exception ex)
        {

            throw;
        }

        return Result.Ok(result);
    }

    //CacheAside
    public async Task<Result<List<News>>> GetAllNewsAsync()
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

            throw;
        }

        return Result.Ok(result);
    }

    //Cache aside
    public async Task<Result<News>> GetNewsByIDAsync(int Id)
    {
        var result = new News();

        try
        {
            var cache = await _newsCache.GetNewsByIDAsync(Id);

            if (cache is null)
            {



            }

        }
        catch (Exception)
        {

            throw;
        }

        return Result.Ok(result);
    }
}
