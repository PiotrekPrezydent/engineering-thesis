using Dara.BuildingBlocks.Contracts;

namespace Dara.Modules.Communication.Contracts.Group.Change;

public record ChangeGroupNameCommand() : IApplicationCommand;

public record ChangeGroupNameCommandResult() : IApplicationCommandResult;