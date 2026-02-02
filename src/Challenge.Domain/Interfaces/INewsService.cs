using Challenge.Domain.Models;
using FluentResults;

namespace Challenge.Domain.Interfaces;

public interface INewsService
{
    public Task<Result<List<News>>> GetAllNewsAsync();
    public Task<Result<News>> GetNewsByIDAsync(int Id);
    public Task<Result<List<News>>> GetNewsTakeNumber(int number);
}
