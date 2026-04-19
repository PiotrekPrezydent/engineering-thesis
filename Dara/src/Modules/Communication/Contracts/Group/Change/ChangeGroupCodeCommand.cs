using Dara.BuildingBlocks.Contracts;

namespace Dara.Modules.Communication.Contracts.Group.Change;

public record ChangeGroupCodeCommand() : IApplicationCommand;

public record ChangeGroupCodeCommandResult() : IApplicationCommandResult;