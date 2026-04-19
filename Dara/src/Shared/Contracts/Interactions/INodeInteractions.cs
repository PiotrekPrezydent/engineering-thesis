namespace Dara.Shared.Contracts.Interactions;

//define every use case for Node
public interface INodeInteractions
{
    public Task ChangeNodeNameAsync();

    public Task ChangeNodeAuthTokenAsync();

    public Task CreateMeshAsync();

    public Task JoinMeshAsync();
    
    public Task LeaveMeshAsync();
}