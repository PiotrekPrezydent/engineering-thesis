using Dara.BuildingBlocks.Application;
using Dara.Modules.Communication.Application.Nodes;
using Dara.Shared.Contracts.Common;
using Dara.Shared.Contracts.Interactions;

namespace Dara.Apps.Server.API.AppHubs;

public partial class AppHub : IGuestInteractions
{
    public async Task<AppResponse> EnableClientNodeAsync(NodeDto nodeDto)
    {
        AppResponse response = new();
        
        var id = Context.ConnectionId;
        var command = new CreateNodeCommand(id, nodeDto.Name, nodeDto.AuthCode);

        var result = await _applicationCommandDispatcher.DispatchAsync<CreateNodeCommand, CreateNodeCommandResult>(command);

        if (result.Status == CommandResultStatus.Success)
        {
            Console.WriteLine("SUCCESS ENABLE");
            response.SetExpectedResponse(nodeDto);
        }
        else
        {
            Console.WriteLine("FAILURE ENABLE");
            response.SetException(new Exception("FAILURE ENABLE"));
        }

        return response;
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
        }
        else
        {
            Console.WriteLine("FAILURE DISABLE");
            response.SetException(new Exception("FAILURE DISABLE"));
        }
        return response;
    }
}