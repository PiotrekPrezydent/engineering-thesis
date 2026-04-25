using Dara.BuildingBlocks.Application.Commands;

namespace Dara.Modules.Connections.Application.Clients.DeleteClient
{
    public record DeleteClientCommand(string ConnectionId) : IModuleCommand;
}