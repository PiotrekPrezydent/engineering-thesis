using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Groups.Domain.Groups;

namespace Dara.Server.Modules.Groups.Application.CreateGroup;

public class CreateGroupCommandHandler : ICommandHandler<CreateGroupCommand>
{
    private readonly IGroupRepository _groupRepository;

    private static int counter = 0;
    
    public CreateGroupCommandHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }
    
    public async Task HandleAsync(CreateGroupCommand command)
    {
        var group = Group.Create(new(command.CreatorId), command.Name, $"GROUP-{counter++}");
        await _groupRepository.AddAsync(group);
    }
}