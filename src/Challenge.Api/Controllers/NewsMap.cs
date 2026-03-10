using Challenge.Application.Dto;
using Challenge.Application.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;


namespace Challenge.Api.Controllers;

public class NewsMap
{
    public static void ExposeMaps(WebApplication app)
    {
        app.MapGet("api/news/", GetAllAsync);

        app.MapGet("api/news/{id}", GetByIdAsync);

        app.MapGet("api/news/first/{number}", GetByNumberAsync);
    }

    public static async Task<Results<Ok<List<HackNewsDto>>, BadRequest<Exception>>> GetAllAsync([FromServices] INewsService service, ILogger<NewsMap> logger)
    {
        try
        {
            var listAll = await service.GetAllNewsAsync();

            return listAll.IsSuccess ? TypedResults.Ok(listAll.Value) : TypedResults.Ok(new List<HackNewsDto>());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"{ex.Message}");
            return TypedResults.BadRequest(ex);
        }

    }

    public static async Task<Results<Ok<List<HackNewsDto>>, BadRequest<Exception>>> GetByNumberAsync([FromServices] INewsService service, [FromRoute] int number, ILogger<NewsMap> logger)
    {
        try
        {
            if (number <= 0)
            {
                return TypedResults.BadRequest(new Exception("Number must be greater than 0 (zero)"));
            }

            var listNews = await service.GetNewsTakeNumber(number);

            return TypedResults.Ok(listNews.Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"{ex.Message}");
            return TypedResults.BadRequest(new Exception(ex.Message));
        }
    }

    public static async Task<Results<Ok<HackNewsDto>, BadRequest<Exception>>> GetByIdAsync([FromServices] INewsService service, string id, ILogger<NewsMap> logger)
    {
        try
        {
            var news = await service.GetNewsByIDAsync(int.Parse(id));
            return TypedResults.Ok(news.Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"{ex.Message}");
            return TypedResults.BadRequest(ex);
        }
    }

}
