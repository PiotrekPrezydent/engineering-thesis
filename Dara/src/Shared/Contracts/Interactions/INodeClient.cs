namespace Dara.Shared.Contracts.Interactions;

public interface INodeClient
{
    public Task ReceiveNodeStateAsync();

    public Task ReceiveMeshListAsync();
}