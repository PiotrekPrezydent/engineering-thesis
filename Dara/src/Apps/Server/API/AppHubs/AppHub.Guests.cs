using Dara.BuildingBlocks.Application;
using Dara.BuildingBlocks.Domain.Exceptions;
using Dara.Modules.Communication.Application.Nodes;
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
        var command = new CreateNodeCommand(id, nodeDto.Name, nodeDto.AuthCode);

        var result = await _applicationCommandDispatcher.DispatchAsync<CreateNodeCommand, CreateNodeCommandResult>(command);

        if (result.Status == CommandResultStatus.Success)
        {
            Console.WriteLine("SUCCESS ENABLE");
            response.SetExpectedResponse(nodeDto);
            
            await Clients.Caller.ReceiveConnectionIdAsync();
        }
        else
        {
            Console.WriteLine("FAILURE ENABLE");
            response.SetException();
            //response.SetException(new Exception("TESTERROR"));
        }
        
        Console.WriteLine($"response from server:::: {response.Status}");
        return response;
    }

    public async Task<string> TestCommand()
    {
        string result = "START";
        result = "TEST SERVER";
        
        return result;
    }

    public async Task<AppResponse> DisableClientNodeAsync()
    {
        AppResponse response = new();
        
        var id = Context.ConnectionId;
        var command = new DeleteNodeCommand(id);
        
        var result = await _applicationCommandDispatcher.DispatchAsync<DeleteNodeCommand, DeleteNodeCommandResult>(command);
        
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