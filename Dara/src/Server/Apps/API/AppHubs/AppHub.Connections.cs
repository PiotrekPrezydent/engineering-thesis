using Dara.Shared.Common;
using Dara.Shared.Contracts.Dtos;
using Dara.Shared.Contracts.Interactions;

namespace Dara.Server.Apps.API.AppHubs;

public partial class AppHub : IConnectionInteractions
{
    public async Task<WrappedResult<MessageDto>> ActiveClientAsync(ClientActivationDto client)
    {
        // var command = new CreateClientCommand(Context.ConnectionId, client.ClientName, client.ClientAuthToken);
        // var result = await _commandRunner.ExecuteAsync(command);
        //
        // MessageDto message = new(result.IsSuccess.ToString());
        // return message;
        return null;
    }

    public async Task<WrappedResult<MessageDto>> DeactiveClientAsync()
    {
        // var command = new DeleteClientCommand(Context.ConnectionId);
        // var result = await _commandRunner.ExecuteAsync(command);
        //
        // MessageDto message = new(result.IsSuccess.ToString());
        // return message;
        return null;
    }
}