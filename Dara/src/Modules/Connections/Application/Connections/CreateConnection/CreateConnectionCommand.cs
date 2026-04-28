using Dara.BuildingBlocks.Application;

namespace Dara.Modules.Connections.Application.Connections.CreateConnection
{
    public record CreateConnectionCommand(string ConnectionId, string ConnectionIp) : IModuleCommand;
}