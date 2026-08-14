using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Groups.Domain.Groups;

namespace Dara.Server.Modules.Groups.Application.Groups.CreateNewGroup;

public class CreateNewGroupCommandHandler : ICommandHandler<CreateNewGroupCommand,Guid>
{
    private readonly IGroupRepository _groupRepository;
    
    
    public CreateNewGroupCommandHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }
    
    public async Task<Guid> HandleAsync(CreateNewGroupCommand command)
    {
        var group = Group.CreateNewGroup(new(command.CreatorId), command.GroupName, command.JoinCode);
        await _groupRepository.AddAsync(group);
        return group.Id;
    }
}