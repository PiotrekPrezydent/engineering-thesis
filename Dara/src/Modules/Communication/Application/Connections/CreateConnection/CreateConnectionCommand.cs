using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.Connections.CreateConnection;

public record CreateConnectionCommand(string ConnectionId, string ConnectionIp) : IApplicationCommand;