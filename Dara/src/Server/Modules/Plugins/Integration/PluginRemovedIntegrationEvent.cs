using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Plugins.Integration;

public record PluginRemovedIntegrationEvent : IntegrationEventBase
{
    public Guid PluginOwnerId { get; }
    public Guid PluginId { get; }
    
    public PluginRemovedIntegrationEvent(Guid eventId, DateTime occurredOn, Guid pluginOwnerId, Guid pluginId) : base(eventId, occurredOn)
    {
        PluginOwnerId = pluginOwnerId;
        PluginId = pluginId;
    }
}