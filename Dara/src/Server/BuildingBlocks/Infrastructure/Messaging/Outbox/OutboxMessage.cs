namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox;

public class OutboxMessage
{
    public Guid Id { get; private set; }
    public DateTime OccurredOn { get; private set; }
    
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string Type { get; private set; }
    
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string Content { get;  private set; }
    
    public DateTime? ProcessedDate { get; set; }
    
    public OutboxMessage(Guid id, DateTime occurredOn, string type, string content)
    {
        Id = id;
        OccurredOn = occurredOn;
        Type = type;
        Content = content;
    }
    
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private OutboxMessage() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}