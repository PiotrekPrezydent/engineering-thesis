using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Connections.Application.Connections.DeleteConnection
{
    public record DeleteConnectionCommand(string ConnectionId) : IApplicationCommand;
}