using BenchmarkDotNet.Attributes;
using Challenge.Api.Controllers;
using Challenge.Application.Dto;
using Challenge.Application.Interfaces;
using Challenge.Application.Services;
using Challenge.Domain.Interfaces;
using Challenge.Domain.Models;
using Challenge.Infra.Client;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Challenge.Tests;

[MemoryDiagnoser]
[TestCaseOrderer("OrdererTypeName", "OrdererAssemblyName")]
public class NewsCacheHandlerTest : IClassFixture<HachNewsFixture>
{
    private readonly ILogger<NewsMap> _loggerNewsMap;
    private readonly ILogger<NewsService> _loggerNewService;
    private readonly HackNewsClient _hackNewsClient;
    private readonly INewsCache _newsCache;
    private readonly InitialLoadHackNewsWorker _initialLoadHackNews;
    public NewsCacheHandlerTest(HachNewsFixture fixture)
    {
        _newsCache = fixture.serviceProvider.GetService<INewsCache>();
        _hackNewsClient = fixture.serviceProvider.GetService<HackNewsClient>(); ;

        var factory = fixture.serviceProvider.GetService<ILoggerFactory>();
        _loggerNewService = factory.CreateLogger<NewsService>();
        _loggerNewsMap = factory.CreateLogger<NewsMap>();

        _initialLoadHackNews = new InitialLoadHackNewsWorker(factory.CreateLogger<InitialLoadHackNewsWorker>(),
            _newsCache,
            _hackNewsClient);

        _initialLoadHackNews.StartAsync(CancellationToken.None).ConfigureAwait(true);
    }

    [Fact(DisplayName = "Get Main Async Integration Hack News")]
    public async Task A_GetAllAsync_MainIntegrationTest()
    {
        Thread.Sleep(10000);
    }

    [Benchmark]
    [Fact(DisplayName = "Get All Async Integration Hack News")]
    public async Task GetAllAsync_IntegrationTest()
    {
        // Arrange
        INewsService service = new NewsService(_loggerNewService, _newsCache, _hackNewsClient);
        var controller = new NewsMap();

        // Act
        Results<Ok<List<HackNewsDto>>, BadRequest<Exception>> result =
            await NewsMap.GetAllAsync(service, _loggerNewsMap);

        // Assert
        Assert.IsType<Ok<List<News>>>(result.Result);
    }

    [Benchmark]
    [Fact(DisplayName = "Get All Async Best Stories Integration  Hack News")]
    public async Task GetAllAsync_BestSotriesIntegrationTest()
    {
        // Arrange
        INewsService service = new NewsService(_loggerNewService, _newsCache, _hackNewsClient);
        var controller = new NewsMap();

        // Act
        var resultBestStories = await _newsCache.GetAllBestStoriesAsync();
        // Assert
        var besties = resultBestStories.Value;
        Ok<HackNewsDto> okResult = null;

        foreach (var item in besties)
        {
            Results<Ok<HackNewsDto>, BadRequest<Exception>> result = await NewsMap.GetByIdAsync(service, item.ToString(), _loggerNewsMap);
            okResult = Assert.IsType<Ok<HackNewsDto>>(result.Result);
        }

        // Assert
        Assert.NotNull(okResult);
    }
}
