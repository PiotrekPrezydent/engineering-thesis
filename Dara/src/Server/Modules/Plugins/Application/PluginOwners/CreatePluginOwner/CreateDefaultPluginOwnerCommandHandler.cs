using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;

namespace Dara.Server.Modules.Plugins.Application.PluginOwners.CreatePluginOwner;

public class CreateDefaultPluginOwnerCommandHandler : ICommandHandler<CreateDefaultPluginOwnerCommand>
{
    private readonly IPluginOwnerRepository _ownersRepository;

    public CreateDefaultPluginOwnerCommandHandler(IPluginOwnerRepository ownersRepository)
    {
        _ownersRepository = ownersRepository;
    }

    public async Task HandleAsync(CreateDefaultPluginOwnerCommand command)
    {
        // the owner exists already when the client registered plugins before this event came
        if (await _ownersRepository.FindByIdAsync(new(command.PluginOwnerId)) is not null)
            return;

        var owner = PluginOwner.CreateDefault(new(command.PluginOwnerId));
        await _ownersRepository.AddAsync(owner);
    }
}