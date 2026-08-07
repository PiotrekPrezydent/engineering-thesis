namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox;

public class InboxMessage
{
    public Guid Id { get; set; }
    public DateTime OccurredOn { get;  set; }
    public DateTime? ProcessedDate { get; set; }
    public string Type { get; set; }
    
    public string Content { get; set; }

    private InboxMessage() { }

    public InboxMessage(Guid id, DateTime occurredOn, string type, string content)
    {
        Id = id;
        OccurredOn = occurredOn;
        Type = type;
        Content = content;
    }
}