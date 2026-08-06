namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox;

public class OutboxMessage
{
    public Guid Id { get; private set; }
    public DateTime OccurredOn { get; private set; }
    public DateTime? ProcessedDate { get; private set; }
    public string MessageType { get; private set; }
    public string MessageContent { get; private set; }
    
    private OutboxMessage() { }
    public OutboxMessage(Guid id, DateTime occurredOn, string messageType, string messageContent)
    {
        Id = id;
        OccurredOn = occurredOn;
        MessageType = messageType;
        MessageContent = messageContent;
    }
    
}