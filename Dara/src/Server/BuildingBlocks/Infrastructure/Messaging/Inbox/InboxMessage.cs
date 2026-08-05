namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox;

public class InboxMessage
{
    public Guid Id { get; }
    public DateTime OccurredOn { get; }
    public DateTime? ProcessedDate { get; }
    
    public string MessageType { get; }
    
    public string IntegrationContent { get; }

    public InboxMessage(Guid id, DateTime occurredOn, string messageType, string integrationContent)
    {
        Id = id;
        OccurredOn = occurredOn;
        MessageType = messageType;
        IntegrationContent = integrationContent;
    }
}