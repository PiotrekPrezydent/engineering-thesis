using Dara.Shared.Contracts.Abstractions;
using Dara.Shared.Contracts.Common;
using Dara.Shared.Contracts.Interactions;

namespace Dara.Apps.Server.API.AppHubs;

public partial class AppHub : INodeInteractions
{
    public Task<AppResponse> ChangeNodeNameAsync()
    {
        throw new NotImplementedException();
    }

    public Task<AppResponse> ChangeNodeAuthTokenAsync()
    {
        throw new NotImplementedException();
    }

    public Task<AppResponse> CreateMeshAsync()
    {
        throw new NotImplementedException();
    }

    public Task<AppResponse> JoinMeshAsync()
    {
        throw new NotImplementedException();
    }

    public Task<AppResponse> LeaveMeshAsync()
    {
        throw new NotImplementedException();
    }
}