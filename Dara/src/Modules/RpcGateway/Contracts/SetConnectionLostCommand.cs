using Dara.BuildingBlocks.Contracts;

namespace Dara.Modules.RpcGateway.Contracts;

public record SetConnectionLostCommand(string ConnectionId) : IApplicationCommand;

public record SetConnectionLostCommandResult(string ConnectionId) : IApplicationCommandResult;
