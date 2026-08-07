namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public DateTime OccurredOn { get; set; }
    public DateTime? ProcessedDate { get; set; }
    public string Type { get; set; }
    public string Content { get;  set; }
    
    private OutboxMessage() { }
    public OutboxMessage(Guid id, DateTime occurredOn, string type, string content)
    {
        Id = id;
        OccurredOn = occurredOn;
        Type = type;
        Content = content;
    }
}