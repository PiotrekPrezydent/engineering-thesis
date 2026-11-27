using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;

namespace Dara.Server.Modules.Plugins.Application.PluginOwners.CreatePluginOwner;

public class CreatePluginOwnerCommandHandler : ICommandHandler<CreatePluginOwnerCommand>
{
    private readonly IPluginOwnerRepository _ownersRepository;

    public CreatePluginOwnerCommandHandler(IPluginOwnerRepository ownersRepository)
    {
        _ownersRepository = ownersRepository;
    }

    public async Task HandleAsync(CreatePluginOwnerCommand command)
    {
        // the owner exists already when the client registered plugins before this event came
        if (await _ownersRepository.FindByIdAsync(new(command.OwnerId)) is not null)
            return;

        var owner = PluginOwner.CreateDefault(new(command.OwnerId));
        await _ownersRepository.AddAsync(owner);
    }
}
