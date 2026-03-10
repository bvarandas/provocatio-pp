namespace Challenge.Domain.Bus;

public abstract class Message
{
    public string MessageType { get; protected set; } = string.Empty;
    public int AggregateId { get; protected set; }

    protected Message()
    {
        MessageType = GetType().Name;
    }
}
