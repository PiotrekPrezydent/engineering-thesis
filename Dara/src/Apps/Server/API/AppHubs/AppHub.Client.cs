using Dara.BuildingBlocks.Infrastructure;
using Dara.Modules.Connections.Application.Clients.CreateClient;
using Dara.Shared.Common;
using Dara.Shared.Contracts.Dtos;
using Dara.Shared.Contracts.Interactions;

namespace Dara.Apps.Server.API.AppHubs;

public partial class AppHub : IClientInteractions
{
    public async Task<WrappedResult<MessageDto>> ChangeClientNameAsync(ClientNameDto clientName)
    {
        var command = new CreateClientCommand(Context.ConnectionId, clientName.ClientName, "123");
        
        throw new NotImplementedException();
    }

    public async Task<WrappedResult<MessageDto>> ChangeClientAuthTokenAsync(ClientAuthTokenDto clientAuthToken)
    {
        throw new NotImplementedException();
    }
}