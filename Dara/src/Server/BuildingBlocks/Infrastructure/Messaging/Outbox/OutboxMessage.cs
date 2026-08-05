namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox;

public class OutboxMessage
{
    public Guid Id { get; }
    public DateTime OccurredOn { get; }
    public DateTime? ProcessedDate { get; }
    public string MessageType { get; }
    public string MessageContent { get; }
    
    public OutboxMessage(Guid id, DateTime occurredOn, string messageType, string messageContent)
    {
        Id = id;
        OccurredOn = occurredOn;
        MessageType = messageType;
        MessageContent = messageContent;
    }

    
}