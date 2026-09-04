using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Orchestration.Domain.Groups;

namespace Dara.Server.Modules.Orchestration.Application.Groups.EditPromptMessagePrefix;

public class EditPromptMessagePrefixCommandHandler : ICommandHandler<EditPromptMessagePrefixCommand>
{
    private readonly IGroupRepository _repository;

    public EditPromptMessagePrefixCommandHandler(IGroupRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(EditPromptMessagePrefixCommand command)
    {
        var group = await _repository.GetByIdAsync(new GroupId(command.GroupId));
        group.EditPromptMessagePrefix(command.PromptMessagePrefix);
    }
}