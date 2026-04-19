using Dara.BuildingBlocks.Contracts;

namespace Dara.Modules.Communication.Contracts.Client.Change;

public record ChangeClientNameCommand() : IApplicationCommand;

public record ChangeClientNameCommandResult() : IApplicationCommandResult;