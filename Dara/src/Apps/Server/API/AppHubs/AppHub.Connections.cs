using Dara.Shared.Common;
using Dara.Shared.Contracts.Dtos;
using Dara.Shared.Contracts.Interactions;

namespace Dara.Apps.Server.API.AppHubs;

public partial class AppHub : IConnectionInteractions
{
    public async Task<WrappedResult<MessageDto>> ActiveClientAsync(ClientActivationDto client)
    {
        throw new NotImplementedException();
    }

    public async Task<WrappedResult<MessageDto>> DeactiveClientAsync()
    {
        throw new NotImplementedException();
    }
}