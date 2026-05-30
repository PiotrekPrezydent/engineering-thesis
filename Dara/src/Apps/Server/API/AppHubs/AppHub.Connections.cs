using Dara.Modules.Connections.Application.Clients.CreateClient;
using Dara.Modules.Connections.Application.Clients.DeleteClient;
using Dara.Shared.Common;
using Dara.Shared.Contracts.Dtos;
using Dara.Shared.Contracts.Interactions;

namespace Dara.Apps.Server.API.AppHubs;

public partial class AppHub : IConnectionInteractions
{
    public async Task<WrappedResult<MessageDto>> ActiveClientAsync(ClientActivationDto client)
    {
        var command = new CreateClientCommand(Context.ConnectionId, client.ClientName, client.ClientAuthToken);
        var result = await _commandRunner.ExecuteAsync(command);

        MessageDto message = new(result.IsSuccess.ToString());
        return message;
    }

    public async Task<WrappedResult<MessageDto>> DeactiveClientAsync()
    {
        var command = new DeleteClientCommand(Context.ConnectionId);
        var result = await _commandRunner.ExecuteAsync(command);

        MessageDto message = new(result.IsSuccess.ToString());
        return message;
    }
}