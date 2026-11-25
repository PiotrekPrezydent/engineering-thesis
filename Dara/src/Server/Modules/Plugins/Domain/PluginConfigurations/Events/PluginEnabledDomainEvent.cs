using Dara.Server.BuildingBlocks.Domain.Events;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Dara.Server.Modules.Plugins.Domain.Plugins;

namespace Dara.Server.Modules.Plugins.Domain.PluginConfigurations.Events;

public record PluginEnabledDomainEvent(PluginConfigurationId PluginConfigurationId, PluginOwnerId PluginOwnerId, PluginId PluginId, int Version) : DomainEventBase;
