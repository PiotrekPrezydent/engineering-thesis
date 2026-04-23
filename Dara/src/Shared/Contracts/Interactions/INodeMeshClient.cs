namespace Dara.Shared.Contracts.Interactions;

public interface INodeMeshClient
{
    public Task ReceiveNodeMeshMembers();

    public Task ReceiveNodeMeshName();

    public Task ReceiveNodeMeshCode();
}