
namespace Challenge.Domain.Bus;

public abstract class Command : Message
{
    public DateTime Timestamp { get; private set; }
    protected Command()
    {
        Timestamp = DateTime.Now;
    }

    public abstract bool IsValid();
}
