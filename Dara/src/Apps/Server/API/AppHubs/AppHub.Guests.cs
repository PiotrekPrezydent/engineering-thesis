using Dara.Shared.Contracts.Interactions;

namespace Dara.Apps.Server.API.AppHubs;

public partial class AppHub : IGuestInteractions
{
    public Task RegisterClientNodeAsync()
    {
        throw new NotImplementedException();
    }

    public Task DeregisterClientNodeAsync()
    {
        throw new NotImplementedException();
    }
}