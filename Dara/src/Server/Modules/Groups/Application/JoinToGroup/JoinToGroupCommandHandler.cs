using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Groups.Domain.Groups;

namespace Dara.Server.Modules.Groups.Application.JoinToGroup;

public class JoinToGroupCommandHandler : ICommandHandler<JoinToGroupCommand>
{
    private readonly IGroupRepository _groupRepository;
    
    public JoinToGroupCommandHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }
    
    public async Task HandleAsync(JoinToGroupCommand command)
    {
        var group = await _groupRepository.GetByIdAsync(new GroupId(command.GroupId));
        group.JoinToGroup(new (command.MemberId), command.JoinCode);
    }
}