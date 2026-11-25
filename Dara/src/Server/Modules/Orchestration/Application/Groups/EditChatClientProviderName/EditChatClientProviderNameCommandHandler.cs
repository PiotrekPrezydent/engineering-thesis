using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Orchestration.Domain.Groups;
using Dara.Server.Modules.Orchestration.Domain.Participants;

namespace Dara.Server.Modules.Orchestration.Application.Groups.EditChatClientProviderName;

public class EditChatClientProviderNameCommandHandler : ICommandHandler<EditChatClientProviderNameCommand>
{
    private readonly IGroupRepository _repository;

    public EditChatClientProviderNameCommandHandler(IGroupRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(EditChatClientProviderNameCommand command)
    {
        var group = await _repository.GetByIdAsync(new GroupId(command.GroupId));
        group.EditChatClientProviderName(new ParticipantId(command.RequesterId), command.ChatClientProviderName);
    }
}