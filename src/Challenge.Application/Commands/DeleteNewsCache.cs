namespace Challenge.Application.Commands;

public class DeleteNewsCache : NewsCommand
{
    public DeleteNewsCache(int id)
    {
        Id = id;
    }
}