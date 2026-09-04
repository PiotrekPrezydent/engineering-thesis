using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Orchestration.Domain.Groups;

namespace Dara.Server.Modules.Orchestration.Application.Groups.EditApiKey;

public class EditApiKeyCommandHandler : ICommandHandler<EditApiKeyCommand>
{
    private readonly IGroupRepository _repository;

    public EditApiKeyCommandHandler(IGroupRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(EditApiKeyCommand command)
    {
        var group = await _repository.GetByIdAsync(new GroupId(command.GroupId));
        group.EditApiKey(command.ApiKey);
    }
}