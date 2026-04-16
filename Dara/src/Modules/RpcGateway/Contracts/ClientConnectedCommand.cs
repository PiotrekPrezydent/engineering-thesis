namespace Dara.Modules.RpcGateway.Contracts;

public record ClientConnectedCommand(string ConnectionId, string ConnectionIp);

public record ClientConnectedCommandResult();
