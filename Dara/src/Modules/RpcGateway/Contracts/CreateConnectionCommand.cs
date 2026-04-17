using Dara.BuildingBlocks.Contracts;

namespace Dara.Modules.RpcGateway.Contracts;

public record CreateConnectionCommand() : IApplicationCommand;

public record CreateConnectionCommandResult() : IApplicationCommandResult;
