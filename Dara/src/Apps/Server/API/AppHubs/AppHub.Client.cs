using Dara.Shared.Common;
using Dara.Shared.Contracts.Dtos;
using Dara.Shared.Contracts.Interactions;

namespace Dara.Apps.Server.API.AppHubs;

public partial class AppHub : IClientInteractions
{
    public async Task<WrappedResult<MessageDto>> ChangeClientNameAsync(ClientNameDto clientName)
    {
        throw new NotImplementedException();
    }

    public async Task<WrappedResult<MessageDto>> ChangeClientAuthTokenAsync(ClientAuthTokenDto clientAuthToken)
    {
        throw new NotImplementedException();
    }
}