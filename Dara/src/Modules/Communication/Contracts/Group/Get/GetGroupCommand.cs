using Dara.BuildingBlocks.Contracts;

namespace Dara.Modules.Communication.Contracts.Group.Get;

public record GetGroupCommand() : IApplicationCommand;

public record GetGroupCommandResult() : IApplicationCommandResult;