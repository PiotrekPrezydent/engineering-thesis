using Dara.BuildingBlocks.Contracts;

namespace Dara.Modules.RpcGateway.Contracts;

public record ChangeClientAuthTokenCommand() : IApplicationCommand;

public record ChangeClientAuthTokenCommandResult() : IApplicationCommandResult;