using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Plugins.Integration;

public record PluginFunctionDeactivatedIntegrationEvent : IntegrationEventBase
{
    public Guid PluginOwnerId { get; }
    public Guid PluginId { get; }
    public Guid PluginFunctionId { get; }
    
    public PluginFunctionDeactivatedIntegrationEvent(Guid eventId, DateTime occurredOn, Guid pluginOwnerId, Guid pluginId, Guid pluginFunctionId) : base(eventId, occurredOn)
    {
        PluginOwnerId = pluginOwnerId;
        PluginId = pluginId;
        PluginFunctionId = pluginFunctionId;
    }
}