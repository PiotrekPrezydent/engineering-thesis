using Dara.Shared.Contracts.Common;
using Dara.Shared.Contracts.Interactions;

namespace Dara.Apps.Server.API.AppHubs;

public partial class AppHub : IGuestInteractions
{
    public Task<AppResponse> EnableClientNodeAsync()
    {
        throw new NotImplementedException();
    }

    public Task<AppResponse> DisableClientNodeAsync()
    {
        throw new NotImplementedException();
    }
}