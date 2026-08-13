using Dara.Server.BuildingBlocks.Domain.Events;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins.Events;

public record PluginFunctionActivatedDomainEvent(PluginOwnerId PluginOwnerId, PluginId PluginId, PluginFunctionId PluginFunctionId)  : DomainEventBase;