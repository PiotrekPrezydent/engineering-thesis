using Dara.BuildingBlocks.Application;
using Dara.BuildingBlocks.Application.Commands;
using Dara.Modules.Connections.Application.Clients.CreateClient;
using Dara.Modules.Connections.Application.Clients.DeleteClient;
using Dara.Modules.Connections.Application.Clients.GetClient;
using Dara.Shared.Contracts.Clients;
using Dara.Shared.Contracts.Common;
using Dara.Shared.Contracts.Interactions;

namespace Dara.Apps.Server.API.AppHubs
{
    public partial class AppHub : IConnectionInteractions
    {
        public async Task<AppResponse> ActiveClientAsync(ClientActivationDto client)
        {
            AppResponse response = new();
        
            string id = Context.ConnectionId;

            var command = new CreateClientCommand(id, client.ClientName, client.ClientAuthToken);
            var result = await _applicationCommandDispatcher.DispatchAsync<CreateClientCommand, CreateClientCommandResult>(command);
        
            if (result.Status != CommandResultStatus.Success)
            {
                Console.WriteLine($"COMMAND RESULT FAILED: {result.ResultedException.Message}");
                response.SetException(result.ResultedException.Message);
            }
            else
            {
                response.SetExpectedResponse("ACTIVETED");
            }

            return response;
        }

        public async Task<AppResponse> DeactiveClientAsync()
        {
            AppResponse response = new();
        
            string id = Context.ConnectionId;
        
            var command = new DeleteClientCommand(id);
            var result = await _applicationCommandDispatcher.DispatchAsync<DeleteClientCommand, DeleteClientCommandResult>(command);
        
            if (result.Status != CommandResultStatus.Success)
            {
                Console.WriteLine($"COMMAND RESULT FAILED: {result.ResultedException.Message}");
                response.SetException(result.ResultedException.Message);
            }
            else
            {
                response.SetExpectedResponse("DEACTIVATED");
            }

            return response;
        }

        bool TryGetConnectionClient(string id, out Guid clientId)
        {
            clientId = Guid.Empty;
            var command = new GetClientCommand(id);
            var result = _applicationCommandDispatcher.DispatchAsync<GetClientCommand, GetClientCommandResult>(command).Result;
        
            if (result.Status != CommandResultStatus.Success)
            {
                return false;
            }
            else
            {
                clientId = result.GetResult<GetClientCommandResult>().ClientId;
                return true;
            }
        }
    }
}