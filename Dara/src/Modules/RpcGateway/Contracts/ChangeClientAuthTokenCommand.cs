using Dara.BuildingBlocks.Domain.Commands;

namespace Dara.Modules.RpcGateway.Contracts;

public record ChangeClientAuthTokenCommand(string AuthToken) : IApplicationCommand;

public record ChangeClientAuthTokenCommandResult() : IApplicationCommandResult;