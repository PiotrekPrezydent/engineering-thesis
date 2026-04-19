namespace Dara.Shared.Contracts.Clients;

public interface INodeMeshClient
{
    public Task ReceiveNodeMeshMembers();

    public Task ReceiveNodeMeshName();

    public Task ReceiveNodeMeshCode();
}