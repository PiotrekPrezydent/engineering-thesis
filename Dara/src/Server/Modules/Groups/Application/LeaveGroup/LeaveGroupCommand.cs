using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Groups.Application.LeaveGroup;

public record LeaveGroupCommand(Guid GroupId, Guid MemberId) : ICommand;
