namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox;

public class InboxMessage
{
    public Guid Id { get; private set; }
    public DateTime OccurredOn { get; private set; }
    public DateTime? ProcessedDate { get; private set; }
    public string Type { get; private set; }
    
    public string Content { get; private set; }

    private InboxMessage() { }

    public InboxMessage(Guid id, DateTime occurredOn, string type, string content)
    {
        Id = id;
        OccurredOn = occurredOn;
        Type = type;
        Content = content;
    }
}