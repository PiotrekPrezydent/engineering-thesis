namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox;

public class InboxMessage
{
    public Guid Id { get; private set; }
    public DateTime OccurredOn { get; private set; }
    public DateTime? ProcessedDate { get; private set; }
    public string MessageType { get; private set; }
    
    public string IntegrationContent { get; private set; }

    private InboxMessage() { }

    public InboxMessage(Guid id, DateTime occurredOn, string messageType, string integrationContent)
    {
        Id = id;
        OccurredOn = occurredOn;
        MessageType = messageType;
        IntegrationContent = integrationContent;
    }
}