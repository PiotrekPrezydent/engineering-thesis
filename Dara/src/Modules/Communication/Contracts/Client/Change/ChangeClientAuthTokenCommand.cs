using Dara.BuildingBlocks.Contracts;

namespace Dara.Modules.Communication.Contracts.Client.Change;

public record ChangeClientAuthTokenCommand(string NewAuthToken) : IApplicationCommand;

public record ChangeClientAuthTokenCommandResult() : IApplicationCommandResult;