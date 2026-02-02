using Challenge.Domain.Interfaces;
using Challenge.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Challenge.Api.Controllers;

public static class NewsMap
{
    public static void ExposeMaps(WebApplication app)
    {
        app.MapGet("api/news/", async ([FromServices] INewsService service, ILogger<Program> logger) =>
        {
            try
            {
                var listAll = await service.GetAllNewsAsync();

                return listAll.IsSuccess ? Results.Ok(listAll.Value) : Results.Ok(new List<News>());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{ex.Message}");
                return Results.BadRequest(ex);
            }
        });

        app.MapGet("api/news/{id}", async ([FromServices] INewsService service, string id, ILogger<Program> logger) =>
        {
            try
            {
                var news = await service.GetNewsByIDAsync(int.Parse(id));
                return Results.Ok(news);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{ex.Message}");
                return Results.BadRequest(ex);
            }
        });

        app.MapGet("api/news/first/{number}", async ([FromServices] INewsService service, [FromRoute] int number, ILogger<Program> logger) =>
        {
            try
            {
                if (number <= 0)
                {
                    return Results.BadRequest("NUmero deveHAckNews  ser maio que 0 (zero)");
                }

                var listNews = await service.GetNewsTakeNumber(number);

                return Results.Ok(listNews.Value);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{ex.Message}");
                return Results.BadRequest(ex);
            }
        });
    }
}
