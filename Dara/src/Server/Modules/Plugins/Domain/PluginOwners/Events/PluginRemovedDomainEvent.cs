using Dara.Server.BuildingBlocks.Domain.Events;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners.Events;

public record PluginRemovedDomainEvent(PluginOwnerId PluginOwnerId, PluginId PluginId)  : DomainEventBase;