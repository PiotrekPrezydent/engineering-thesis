using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Connections.Application.Clients.DeleteClient
{
    public record DeleteClientCommand(string ConnectionId) : IApplicationCommand;
}