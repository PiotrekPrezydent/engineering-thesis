using Dara.Shared.Contracts.Common;

namespace Dara.Shared.Contracts.Interactions;

public interface INodeMeshInteractions
{
    public Task<AppResponse> ChangeMeshNameAsync();

    public Task<AppResponse> ChangeMeshCodeAsync();
}