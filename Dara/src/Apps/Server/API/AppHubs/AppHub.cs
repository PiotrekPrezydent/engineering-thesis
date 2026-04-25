using Dara.BuildingBlocks.Application;
using Dara.BuildingBlocks.Application.Commands;
using Dara.BuildingBlocks.Infrastructure.Commands;
using Dara.Modules.Connections.Application.Connections.CreateConnection;
using Dara.Modules.Connections.Application.Connections.DeleteConnection;
using Dara.Shared.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Apps.Server.API.AppHubs
{
    public partial class AppHub : Hub<IAppHubClient>, IAppHub
    {
        private readonly IApplicationCommandDispatcher _applicationCommandDispatcher;
    
        public AppHub(IApplicationCommandDispatcher applicationCommandDispatcher)
        {
            _applicationCommandDispatcher = applicationCommandDispatcher;
        }
    
        public override async Task OnConnectedAsync()
        {
            string id = Context.ConnectionId;
            string ip = Context.GetHttpContext()!.Connection.RemoteIpAddress!.ToString();

            var command = new CreateConnectionCommand(id, ip);
            var result = await _applicationCommandDispatcher.DispatchAsync<CreateConnectionCommand, CreateConnectionCommandResult>(command);

            if (result.Status != CommandResultStatus.Success)
            {
                Console.WriteLine($"COMMAND RESULT FAILED: {result.ResultedException.Message}");
            }
        
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string id = Context.ConnectionId;

            var command = new DeleteConnectionCommand(id);
            var result = await _applicationCommandDispatcher.DispatchAsync<DeleteConnectionCommand, DeleteConnectionCommandResult>(command);
        
            if (result.Status != CommandResultStatus.Success)
            {
                Console.WriteLine($"COMMAND RESULT FAILED: {result.ResultedException.Message}");
            }
        
            await base.OnDisconnectedAsync(exception);
        }
    
    }
}