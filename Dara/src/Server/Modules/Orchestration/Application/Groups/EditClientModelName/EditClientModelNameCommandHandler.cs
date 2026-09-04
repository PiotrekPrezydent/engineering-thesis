using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Orchestration.Domain.Groups;

namespace Dara.Server.Modules.Orchestration.Application.Groups.EditClientModelName;

public class EditClientModelNameCommandHandler : ICommandHandler<EditClientModelNameCommand>
{
    private readonly IGroupRepository _repository;

    public EditClientModelNameCommandHandler(IGroupRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(EditClientModelNameCommand command)
    {
        var group = await _repository.GetByIdAsync(new GroupId(command.GroupId));
        group.EditClientModelName(command.ClientModelName);
    }
}