using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Groups.Domain.Groups;

namespace Dara.Server.Modules.Groups.Application.Groups.LeaveGroup;

public class LeaveGroupCommandHandler : ICommandHandler<LeaveGroupCommand>
{
    private readonly IGroupRepository _groupRepository;

    public LeaveGroupCommandHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task HandleAsync(LeaveGroupCommand command)
    {
        var group = await _groupRepository.GetByIdAsync(new(command.GroupId));
        group.LeaveGroup(new (command.MemberId));
    }
}