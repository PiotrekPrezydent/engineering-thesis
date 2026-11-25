using Dara.Server.BuildingBlocks.Domain.Events;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;

namespace Dara.Server.Modules.Plugins.Domain.Plugins.Events;

// carries the function ids because handlers run before the plugin is saved
public record PluginAddedDomainEvent(PluginOwnerId PluginOwnerId, PluginId PluginId, IReadOnlyList<PluginFunctionId> FunctionIds) : DomainEventBase;
