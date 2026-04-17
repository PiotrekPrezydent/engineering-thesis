using Dara.BuildingBlocks.Contracts;

namespace Dara.Modules.RpcGateway.Contracts;

public record ChangeClientAuthTokenCommand(string AuthToken) : IApplicationCommand;

public record ChangeClientAuthTokenCommandResult() : IApplicationCommandResult;