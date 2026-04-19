namespace Dara.Shared.Contracts.Interactions;

//define every use case fore connections
public interface IGuestInteractions
{
    public Task RegisterClientNodeAsync();
    
    public Task DeregisterClientNodeAsync();
}