using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Connections.Application.Connections.CreateConnection
{
    public record CreateConnectionCommand(string ConnectionId, string ConnectionIp) : IApplicationCommand;
}