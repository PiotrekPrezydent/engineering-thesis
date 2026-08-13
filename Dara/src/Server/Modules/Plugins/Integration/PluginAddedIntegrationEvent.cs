using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Plugins.Integration;

public record PluginAddedIntegrationEvent : IntegrationEventBase
{
    public Guid PluginOwnerId { get; }
    public PluginSnapshot PluginSnapshot { get; }
    
    public PluginAddedIntegrationEvent(Guid eventId, DateTime occurredOn, Guid pluginOwnerId, PluginSnapshot pluginSnapshot) : base(eventId, occurredOn)
    {
        PluginSnapshot = pluginSnapshot;
        PluginOwnerId = pluginOwnerId;
    }
}