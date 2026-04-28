using Dara.BuildingBlocks.Application;

namespace Dara.Modules.Connections.Application.Clients.DeleteClient
{
    public record DeleteClientCommand(string ConnectionId) : IModuleCommand;
}