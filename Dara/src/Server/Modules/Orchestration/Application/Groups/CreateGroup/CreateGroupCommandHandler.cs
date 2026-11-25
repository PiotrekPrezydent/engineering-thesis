using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Orchestration.Domain.Groups;
using Dara.Server.Modules.Orchestration.Domain.Participants;

namespace Dara.Server.Modules.Orchestration.Application.Groups.CreateGroup;

public class CreateGroupCommandHandler : ICommandHandler<CreateGroupCommand>
{
    private readonly IGroupRepository _repository;

    public CreateGroupCommandHandler(IGroupRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(CreateGroupCommand command)
    {
        var group = Group.CreateDefault(new GroupId(command.GroupId), new ParticipantId(command.OwnerId));
        await _repository.AddAsync(group);
    }
}