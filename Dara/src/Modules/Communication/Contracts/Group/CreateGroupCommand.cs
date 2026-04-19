using Dara.BuildingBlocks.Contracts;

namespace Dara.Modules.Communication.Contracts.Group;

public record CreateGroupCommand() : IApplicationCommand;

public record CreateGroupCommandResult() : IApplicationCommandResult;