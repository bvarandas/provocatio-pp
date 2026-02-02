
using Challenge.Application.Commands;
using Challenge.Domain.Bus;
using Challenge.Domain.Interfaces;
using Challenge.Domain.Models;
using Challenge.Domain.Notifications;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Challenge.Application.Handlers;

public class NewsCacheHandler : CommandHandler,
    IRequestHandler<DeleteNewsCache, Result<bool>>,
    IRequestHandler<InsertNewsCache, Result<bool>>
{
    private readonly INewsCache _newsCache;
    private readonly ILogger<NewsCacheHandler> _logger;
    public NewsCacheHandler(
        INewsCache newsCache,
        IMediatorHandler bus,
        ILogger<NewsCacheHandler> logger,
        INotificationHandler<DomainNotification> notifications) : base(bus, notifications)
    {
        _newsCache = newsCache;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(DeleteNewsCache command, CancellationToken cancellationToken)
    {
        var news = new News() { Id = command.Id };

        var result = await _newsCache.RemoveNewsAsync(news);

        if (!result.IsSuccess)
        {
            _logger.LogError("error on insert hacker news on cache");

            return await Task.FromResult(false);
        }

        return await Task.FromResult(true);
    }

    public async Task<Result<bool>> Handle(InsertNewsCache command, CancellationToken cancellationToken)
    {
        var news = new News(command.By, command.Descendants, command.Kids, command.Score, command.Title, command.Type, command.Type);

        news.Time = command.Time;

        var result = await _newsCache.UpsertNewsAsync(news);

        if (!result.IsSuccess)
        {
            _logger.LogError("error on insert hacker news on cache");

            return await Task.FromResult(false);
        }

        return await Task.FromResult(true);
    }
}
