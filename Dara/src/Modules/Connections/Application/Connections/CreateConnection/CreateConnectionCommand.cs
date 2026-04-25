using Dara.BuildingBlocks.Application.Commands;

namespace Dara.Modules.Connections.Application.Connections.CreateConnection
{
    public record CreateConnectionCommand(string ConnectionId, string ConnectionIp) : IModuleCommand;
}