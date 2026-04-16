using Dara.BuildingBlocks.Domain.Commands;

namespace Dara.Modules.RpcGateway.Contracts;

public record ClientConnectedCommand(string ConnectionId, string ConnectionIp) : IApplicationCommand;

public record ClientConnectedCommandResult() : IApplicationCommandResult;
