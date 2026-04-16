namespace Dara.Modules.RpcGateway.Contracts;

public record ClientDisconnectedCommand(string ConnectionId);

public record ClientDisconnectedCommandResult(string ConnectionId);