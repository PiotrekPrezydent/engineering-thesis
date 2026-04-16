using Dara.BuildingBlocks.Contracts;

namespace Dara.Modules.RpcGateway.Contracts;

public record ClientDisconnectedCommand(string ConnectionId) : IApplicationCommand;

public record ClientDisconnectedCommandResult(string ConnectionId) : IApplicationCommandResult;