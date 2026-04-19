namespace Dara.Shared.Contracts.Clients;

public interface INodeClient
{
    public Task ReceiveNodeStateAsync();

    public Task ReceiveMeshListAsync();
}