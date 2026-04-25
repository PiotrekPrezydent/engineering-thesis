using Dara.BuildingBlocks.Application;
using Dara.Modules.Connections.Application.Clients.ChangeClientAuthToken;
using Dara.Modules.Connections.Application.Clients.ChangeClientName;
using Dara.Shared.Contracts.Clients;
using Dara.Shared.Contracts.Common;
using Dara.Shared.Contracts.Interactions;

namespace Dara.Apps.Server.API.AppHubs
{
    public partial class AppHub : IActiveConnectionInteractions
    {
        public async Task<AppResponse> ChangeClientNameAsync(ClientNameDto clientName)
        {
            AppResponse response = new();
        
            string id = Context.ConnectionId;
        
            TryGetConnectionClient(id, out Guid clientId);
        
            var command = new ChangeClientNameCommand(clientId, clientName.ClientName);
            var result = await _applicationCommandDispatcher.DispatchAsync<ChangeClientNameCommand, ChangeClientNameCommandResult>(command);
        
            if (result.Status != CommandResultStatus.Success)
            {
                Console.WriteLine($"COMMAND RESULT FAILED: {result.ResultedException.Message}");
                response.SetException(result.ResultedException.Message);
            }
            else
            {
                response.SetExpectedResponse("NAME CHANGED");
            }

            return response;
        }

        public async Task<AppResponse> ChangeClientAuthTokenAsync(ClientAuthTokenDto clientAuthToken)
        {
            AppResponse response = new();
        
            string id = Context.ConnectionId;
        
            TryGetConnectionClient(id, out Guid clientId);
        
            var command = new ChangeClientAuthTokenCommand(clientId, clientAuthToken.ClientAuthToken);
            var result = await _applicationCommandDispatcher.DispatchAsync<ChangeClientAuthTokenCommand, ChangeClientAuthTokenCommandResult>(command);
        
            if (result.Status != CommandResultStatus.Success)
            {
                Console.WriteLine($"COMMAND RESULT FAILED: {result.ResultedException.Message}");
                response.SetException(result.ResultedException.Message);
            }
            else
            {
                response.SetExpectedResponse("TOKEN CHANGED");
            }

            return response;
        }
    }
}