using Dara.Server.BuildingBlocks.Domain.Events;
using Dara.Server.Modules.Plugins.Domain.Plugins;

namespace Dara.Server.Modules.Plugins.Domain.PluginConfigurations.Events;

public record PluginEnabledDomainEvent(PluginId PluginId) : DomainEventBase;