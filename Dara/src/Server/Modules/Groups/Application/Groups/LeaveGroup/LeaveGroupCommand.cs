using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Groups.Application.Groups.LeaveGroup;

public record LeaveGroupCommand(Guid GroupId, Guid MemberId) : ICommand;
