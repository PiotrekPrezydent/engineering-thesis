using Dara.BuildingBlocks.Application;

namespace Dara.Modules.Connections.Application.Connections.DeleteConnection
{
    public record DeleteConnectionCommand(string ConnectionId) : IModuleCommand;
}