using Challenge.Domain.Models.Results;
using Challenge.Application.Commands;
namespace Challenge.Domain.Interfaces;
public interface INewsCacheHandler
{
    Task<Result<bool>> HandleRemove(DeleteNewsCache command, CancellationToken cancellationToken);
    Task<Result<bool>> HandleUpsert(InsertNewsCache command, CancellationToken cancellationToken);
}