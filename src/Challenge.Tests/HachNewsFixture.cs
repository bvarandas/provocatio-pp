using Challenge.Application.Handlers;
using Challenge.Application.Interfaces;
using Challenge.Application.Services;
using Challenge.Domain.Interfaces;
using Challenge.Infra.Cache;
using Challenge.Infra.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Challenge.Tests;

public class HachNewsFixture
{
    public ServiceProvider serviceProvider { get; private set; }

    public HachNewsFixture()
    {
        var services = new ServiceCollection();

        services.AddSingleton<INewsCacheHandler, NewsCacheHandler>();
        services.AddSingleton<INewsService, NewsService>();
        services.AddSingleton<INewsCache, NewsCache>();
        services.AddHttpClient<HackNewsClient>(client => client.BaseAddress = new Uri("https://hacker-news.firebaseio.com/"));

        serviceProvider = services
            .AddLogging()
            .BuildServiceProvider();
    }
}
