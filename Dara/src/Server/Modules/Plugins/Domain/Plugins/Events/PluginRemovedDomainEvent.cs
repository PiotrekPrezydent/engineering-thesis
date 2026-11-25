using Dara.Server.BuildingBlocks.Domain.Events;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;

namespace Dara.Server.Modules.Plugins.Domain.Plugins.Events;

public record PluginRemovedDomainEvent(PluginOwnerId PluginOwnerId, PluginId PluginId) : DomainEventBase;
