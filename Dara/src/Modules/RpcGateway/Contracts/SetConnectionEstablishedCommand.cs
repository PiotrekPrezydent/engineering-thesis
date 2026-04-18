using Dara.BuildingBlocks.Contracts;

namespace Dara.Modules.RpcGateway.Contracts;

public record SetConnectionEstablishedCommand(string ConnectionId, string ConnectionIp) : IApplicationCommand;

public record SetConnectionEstablishedCommandResult() : IApplicationCommandResult;
