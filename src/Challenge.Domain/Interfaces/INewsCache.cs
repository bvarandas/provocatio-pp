using Challenge.Domain.Models;
using Challenge.Domain.Models.Results;


namespace Challenge.Domain.Interfaces;

public interface INewsCache
{
    public Task<Result> UpsertNewsAsync(News news);
    public Task<Result> RemoveNewsAsync(News news);
    public Task<Result> UpsertBestStoriesAsync(List<int> litBestStories);
    public Task<Result<IEnumerable<int>>> GetAllBestStoriesAsync();
    public Task<Result<IEnumerable<News>>> GetAllNewsAsync();
    public Task<Result<News>> GetNewsByIDAsync(int id);
}