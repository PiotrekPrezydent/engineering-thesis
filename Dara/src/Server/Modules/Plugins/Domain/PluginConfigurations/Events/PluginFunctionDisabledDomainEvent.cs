using Dara.Server.BuildingBlocks.Domain.Events;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Dara.Server.Modules.Plugins.Domain.Plugins;

namespace Dara.Server.Modules.Plugins.Domain.PluginConfigurations.Events;

public record PluginFunctionDisabledDomainEvent(PluginConfigurationId PluginConfigurationId, PluginOwnerId PluginOwnerId, PluginId PluginId, PluginFunctionId PluginFunctionId, int Version) : DomainEventBase;
