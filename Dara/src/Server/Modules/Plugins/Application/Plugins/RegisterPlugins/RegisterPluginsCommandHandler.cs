using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Plugins.Domain.PluginConfigurations;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Dara.Server.Modules.Plugins.Domain.Plugins;

namespace Dara.Server.Modules.Plugins.Application.Plugins.RegisterPlugins;

public class RegisterPluginsCommandHandler : ICommandHandler<RegisterPluginsCommand>
{
    private readonly IPluginOwnerRepository _pluginOwnerRepository;
    private readonly IPluginRepository _pluginRepository;
    private readonly IPluginConfigurationRepository _pluginConfigurationRepository;

    public RegisterPluginsCommandHandler(IPluginOwnerRepository pluginOwnerRepository, IPluginRepository pluginRepository, IPluginConfigurationRepository pluginConfigurationRepository)
    {
        _pluginOwnerRepository = pluginOwnerRepository;
        _pluginRepository = pluginRepository;
        _pluginConfigurationRepository = pluginConfigurationRepository;
    }

    // the same hash changes nothing, a plugin missing from the set goes away, a new hash is a new plugin
    public async Task HandleAsync(RegisterPluginsCommand command)
    {
        var ownerId = new PluginOwnerId(command.OwnerId);
        var owner = await GetOrCreateOwnerAsync(ownerId);
        var owned = (await _pluginRepository.GetByOwnerIdAsync(ownerId)).ToList();

        var registeredHashes = command.Plugins.Select(p => p.Sha256).ToHashSet(StringComparer.OrdinalIgnoreCase);

        // a plugin with the same name and a new hash is missing too, so its new version can take the name
        foreach (var missing in owned.Where(p => !registeredHashes.Contains(p.Hash.Value)).ToList())
        {
            owner.RemovePlugin(owned, missing);
            owned.Remove(missing);
            await _pluginRepository.RemoveAsync(missing);

            var configuration = await _pluginConfigurationRepository.GetByPluginIdAsync(missing.Id);
            configuration.Remove();
            await _pluginConfigurationRepository.RemoveAsync(configuration);
        }

        var ownedHashes = owned.Select(p => p.Hash.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var manifest in command.Plugins.Where(p => !ownedHashes.Contains(p.Sha256)))
        {
            var plugin = owner.AddPlugin(owned, manifest.Name, manifest.Description, manifest.Version,
                new PluginHash(manifest.Sha256), manifest.Functions.Select(PluginManifestMapper.ToDomain).ToList());

            owned.Add(plugin);
            await _pluginRepository.AddAsync(plugin);
        }
    }

    // a client can register before the new user event reaches this module
    private async Task<PluginOwner> GetOrCreateOwnerAsync(PluginOwnerId ownerId)
    {
        var owner = await _pluginOwnerRepository.FindByIdAsync(ownerId);
        if (owner is not null)
            return owner;

        owner = PluginOwner.CreateDefault(ownerId);
        await _pluginOwnerRepository.AddAsync(owner);

        return owner;
    }
}
