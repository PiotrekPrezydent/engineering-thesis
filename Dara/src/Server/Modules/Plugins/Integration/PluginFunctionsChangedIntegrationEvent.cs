using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Plugins.Integration;

// the whole current state of one plugin's functions, empty when the plugin is disabled or removed
public record PluginFunctionsChangedIntegrationEvent : IntegrationEventBase
{
    public Guid PluginOwnerId { get; init; }
    public Guid PluginId { get; init; }

    // grows with every change of the plugin, an older state can come after a newer one
    public int Version { get; init; }

    public IReadOnlyList<AvailableFunctionSnapshot> Functions { get; init; }

    public PluginFunctionsChangedIntegrationEvent(Guid eventId, DateTime occurredOn, Guid pluginOwnerId, Guid pluginId, int version, IReadOnlyList<AvailableFunctionSnapshot> functions) : base(eventId, occurredOn)
    {
        PluginOwnerId = pluginOwnerId;
        PluginId = pluginId;
        Version = version;
        Functions = functions;
    }
}
