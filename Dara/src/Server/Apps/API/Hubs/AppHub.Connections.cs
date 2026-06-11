using Dara.Shared.Common.Results;

namespace Dara.Server.Apps.API.Hubs;

public partial class AppHub : IConnectionInteractions
{
    public async Task<Result<MessageDto>> ActiveClientAsync(ClientActivationDto client)
    {
        // var command = new CreateClientCommand(Context.ConnectionId, client.ClientName, client.ClientAuthToken);
        // var result = await _commandRunner.ExecuteAsync(command);
        //
        // MessageDto message = new(result.IsSuccess.ToString());
        // return message;
        return null;
    }

    public async Task<Result<MessageDto>> DeactiveClientAsync()
    {
        // var command = new DeleteClientCommand(Context.ConnectionId);
        // var result = await _commandRunner.ExecuteAsync(command);
        //
        // MessageDto message = new(result.IsSuccess.ToString());
        // return message;
        return null;
    }
}