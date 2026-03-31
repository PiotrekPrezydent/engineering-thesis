namespace LADR.SharedKernel.Integration.Models;

public class IntegrationEvent
{
    public Guid Id { get; }

    public DateTime Timestamp { get; }
    
    public IntegrationEvent()
    {
        Id = Guid.NewGuid();
        Timestamp = DateTime.Now;
    }

}