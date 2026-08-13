using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Groups.Domain.Groups;

namespace Dara.Server.Modules.Groups.Application.Groups.CreateGroup;

public class CreateGroupCommandHandler : ICommandHandler<CreateGroupCommand,Guid>
{
    private readonly IGroupRepository _groupRepository;
    
    
    public CreateGroupCommandHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }
    
    public async Task<Guid> HandleAsync(CreateGroupCommand command)
    {
        var group = Group.Create(new(command.CreatorId), command.GroupName, command.JoinCode);
        await _groupRepository.AddAsync(group);
        return group.Id;
    }
}