using Dara.Server.BuildingBlocks.Domain.Events;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners.Events;

public record PluginOwnerCreatedDomainEvent(PluginOwnerId PluginOwnerId) : DomainEventBase;