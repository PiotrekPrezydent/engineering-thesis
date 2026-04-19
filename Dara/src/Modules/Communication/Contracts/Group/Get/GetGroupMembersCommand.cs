using Dara.BuildingBlocks.Contracts;

namespace Dara.Modules.Communication.Contracts.Group.Get;

public record GetGroupMembersCommand() : IApplicationCommand;

public record GetGroupMembersCommandResult() : IApplicationCommandResult;