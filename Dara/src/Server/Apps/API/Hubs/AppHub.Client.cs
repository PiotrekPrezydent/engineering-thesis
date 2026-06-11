// using Dara.Shared.Common.Results;
//
// namespace Dara.Server.Apps.API.Hubs;
//
// public partial class AppHub : IClientInteractions
// {
//     public async Task<Result<MessageDto>> ChangeClientNameAsync(ClientNameDto clientName)
//     {
//         // var getClient = new GetClientCommand(Context.ConnectionId);
//         // var getClientResult = await _commandRunner.ExecuteAsync<GetClientCommand, GetClientCommandResult>(getClient);
//         // if (!getClientResult.IsSuccess)
//         // {
//         //     return new MessageDto("No client for connection");
//         // }
//         //
//         // var changeClientName = new ChangeClientNameCommand(getClientResult.Value.ClientId, clientName.ClientName);
//         // var changeCLientNameResult = await _commandRunner.ExecuteAsync(changeClientName);
//         //
//         // return new MessageDto(changeCLientNameResult.IsSuccess.ToString());
//         return null;
//     }
//
//     public async Task<Result<MessageDto>> ChangeClientAuthTokenAsync(ClientAuthTokenDto clientAuthToken)
//     {
//         // var getClient = new GetClientCommand(Context.ConnectionId);
//         // var getClientResult = await _commandRunner.ExecuteAsync<GetClientCommand, GetClientCommandResult>(getClient);
//         // if (!getClientResult.IsSuccess)
//         // {
//         //     return new MessageDto("No client for connection");
//         // }
//         //
//         // var changeClientAuthToken = new ChangeClientAuthTokenCommand(getClientResult.Value.ClientId, clientAuthToken.ClientAuthToken);
//         // var changeClientAuthTokenResult = await _commandRunner.ExecuteAsync(changeClientAuthToken);
//         //
//         // return new MessageDto(changeClientAuthTokenResult.ToString());
//         return null;
//     }
// }