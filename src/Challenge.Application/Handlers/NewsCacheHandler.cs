
using Challenge.Application.Commands;
using Challenge.Domain.Interfaces;
using Challenge.Domain.Models;
using Challenge.Domain.Models.Results;

using Microsoft.Extensions.Logging;

namespace Challenge.Application.Handlers;

public class NewsCacheHandler : CommandHandler, INewsCacheHandler
{
    private readonly INewsCache _newsCache;
    private readonly ILogger<NewsCacheHandler> _logger;
    public NewsCacheHandler(
        INewsCache newsCache,
        ILogger<NewsCacheHandler> logger) : base()
    {
        _newsCache = newsCache;
        _logger = logger;
    }

    public async Task<Result<bool>> HandleRemove(DeleteNewsCache command, CancellationToken cancellationToken)
    {
        var news = new News() { Id = command.Id };

        var result = await _newsCache.RemoveNewsAsync(news);

        if (!result.IsSuccess)
        {
            string error = "error on remove hacker news on cache";
            _logger.LogError(error);

            return await Task.FromResult(Result<bool>.Fail(error));
        }

        return await Task.FromResult(Result<bool>.Ok(true));
    }

    public async Task<Result<bool>> HandleUpsert(InsertNewsCache command, CancellationToken cancellationToken)
    {
        var news = new News(command.By, command.Descendants, command.Kids, command.Score, command.Title, command.Type, command.Type);

        news.Time = command.Time;

        var result = await _newsCache.UpsertNewsAsync(news);

        if (!result.IsSuccess)
        {

            string erro = "error on insert hacker news on cache";
            _logger.LogError(erro);

            return await Task.FromResult(Result<bool>.Fail(erro));
        }

        return await Task.FromResult(Result<bool>.Ok(true));
    }
}
