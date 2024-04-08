namespace BuildingBlock.Messaging;

public class BaseIntegrationEvent(Guid id, DateTime createdAt)
{
    public Guid Id { get; private set; } = id;

    public DateTime CreatedAt { get; private set; } = createdAt;

    public BaseIntegrationEvent() : this(Guid.NewGuid(), DateTime.Now)
    { }
}