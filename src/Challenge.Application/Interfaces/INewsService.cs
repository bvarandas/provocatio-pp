using Challenge.Application.Dto;
using Challenge.Domain.Models.Results;

namespace Challenge.Application.Interfaces;

public interface INewsService
{
    public Task<Result<List<HackNewsDto>>> GetAllNewsAsync();
    public Task<Result<HackNewsDto>> GetNewsByIDAsync(int Id);
    public Task<Result<List<HackNewsDto>>> GetNewsTakeNumber(int number);
}
