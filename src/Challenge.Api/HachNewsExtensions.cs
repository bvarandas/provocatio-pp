using Challenge.Application.Dto;
using Challenge.Domain.Models;

namespace Challenge.Api;

public static class HachNewsExtensions
{
    public static HackNewsDto ToDto(this News news)
    {
        return new HackNewsDto
        {
            Title = news.Title,
            Uri = news.Url,
            PostedBy = news.By,
            Time = DateTimeOffset.FromUnixTimeMilliseconds(news.Time).DateTime,
            Score = news.Score,
            CommentCount = news.Kids?.Count ?? 0
        };
    }

    public static List<HackNewsDto> ToDto(this List<News> listNews)
    {
        var list = new List<HackNewsDto>();
        foreach (var news in listNews)
        {
            new HackNewsDto
            {
                Title = news.Title,
                Uri = news.Url,
                PostedBy = news.By,
                Time = DateTimeOffset.FromUnixTimeMilliseconds(news.Time).DateTime,
                Score = news.Score,
                CommentCount = news.Kids?.Count ?? 0
            };
        }

        return list;
    }
}
