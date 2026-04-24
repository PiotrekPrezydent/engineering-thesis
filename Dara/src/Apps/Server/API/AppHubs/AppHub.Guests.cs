using Dara.BuildingBlocks.Application;
using Dara.Modules.Communication.Application.Clients.CreateClient;
using Dara.Modules.Communication.Application.Clients.DeleteClient;
using Dara.Shared.Contracts.Common;
using Dara.Shared.Contracts.Interactions;

namespace Dara.Apps.Server.API.AppHubs;

public partial class AppHub : IGuestInteractions
{
    public async Task<AppResponse> EnableClientNodeAsync(NodeDto nodeDto)
    {
        AppResponse response = new();
        response.StatusS = "TESTINGDWADWA";
        
        var id = Context.ConnectionId;
        
        var command = new CreateClientCommand(id, nodeDto.Name, nodeDto.AuthCode);
        var result = await _applicationCommandDispatcher.DispatchAsync<CreateClientCommand, CreateClientCommandResult>(command);

        if (result.Status == CommandResultStatus.Success)
        {
            Console.WriteLine("SUCCESS ENABLE");
            response.SetExpectedResponse(nodeDto);
            
            await Clients.Caller.ReceiveConnectionIdAsync();
        }
        else
        {
            Console.WriteLine("FAILURE ENABLE");
            Console.WriteLine(result.ResultedException.Message);
            response.SetException();
            //response.SetException(new Exception("TESTERROR"));
        }
        
        Console.WriteLine($"response from server:::: {response.Status}");
        return response;
    }

    public async Task<AppResponse> DisableClientNodeAsync()
    {
        AppResponse response = new();
        
        var id = Context.ConnectionId;
        var command = new DeleteClientCommand(id);
        
        var result = await _applicationCommandDispatcher.DispatchAsync<DeleteClientCommand, DeleteClientCommandResult>(command);
        
        if (result.Status == CommandResultStatus.Success)
        {
            Console.WriteLine("SUCCESS DISABLE");
            response.SetExpectedResponse(new NodeDto("A","B"));
            
            await Clients.Caller.ReceiveAllConnectionsFromIpAsync();
        }
        else
        {
            Console.WriteLine("FAILURE DISABLE");
           // response.SetException(result.ResultedException);
        }
        return response;
    }
}