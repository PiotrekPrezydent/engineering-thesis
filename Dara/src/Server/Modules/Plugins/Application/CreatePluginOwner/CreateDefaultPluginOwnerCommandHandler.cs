using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;

namespace Dara.Server.Modules.Plugins.Application.CreatePluginOwner;

public class CreateDefaultPluginOwnerCommandHandler : ICommandHandler<CreateDefaultPluginOwnerCommand>
{
    private readonly IPluginOwnerRepository _ownersRepository;

    public CreateDefaultPluginOwnerCommandHandler(IPluginOwnerRepository ownersRepository)
    {
        _ownersRepository = ownersRepository;
    }

    public async Task HandleAsync(CreateDefaultPluginOwnerCommand command)
    {
        var owner = PluginOwner.CreateDefault(new(command.PluginOwnerId));
        await _ownersRepository.AddAsync(owner);
    }
}