using Dara.BuildingBlocks.Contracts;

namespace Dara.Modules.RpcGateway.Contracts;

public record ClientConnectedCommand(string ConnectionId, string ConnectionIp) : IApplicationCommand;

public record ClientConnectedCommandResult() : IApplicationCommandResult;
