using Dara.Shared.Contracts.Common;

namespace Dara.Shared.Contracts.Interactions;

//define every use case for Node
public interface INodeInteractions
{
    public Task<AppResponse> ChangeNodeNameAsync();

    public Task<AppResponse> ChangeNodeAuthTokenAsync();

    public Task<AppResponse> CreateMeshAsync();

    public Task<AppResponse> JoinMeshAsync();
    
    public Task<AppResponse> LeaveMeshAsync();
}