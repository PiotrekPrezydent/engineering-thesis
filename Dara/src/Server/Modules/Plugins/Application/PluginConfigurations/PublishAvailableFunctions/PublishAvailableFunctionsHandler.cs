using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Plugins.Application.Plugins;
using Dara.Server.Modules.Plugins.Domain.PluginConfigurations;
using Dara.Server.Modules.Plugins.Domain.Plugins;
using Dara.Server.Modules.Plugins.Integration;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Plugins.Application.PluginConfigurations.PublishAvailableFunctions;

// runs after the change is saved and publishes the state the plugin has now, a late notification repeats a newer state
public class PublishAvailableFunctionsHandler :
    IDomainEventNotificationHandler<PluginAddedNotification>,
    IDomainEventNotificationHandler<PluginEnabledNotification>,
    IDomainEventNotificationHandler<PluginDisabledNotification>,
    IDomainEventNotificationHandler<PluginFunctionEnabledNotification>,
    IDomainEventNotificationHandler<PluginFunctionDisabledNotification>,
    IDomainEventNotificationHandler<PluginConfigurationRemovedNotification>
{
    private readonly IReadModel _readModel;
    private readonly IEventBus _eventBus;

    public PublishAvailableFunctionsHandler(IReadModel readModel, IEventBus eventBus)
    {
        _readModel = readModel;
        _eventBus = eventBus;
    }

    public async Task HandleAsync(PluginAddedNotification notification)
    {
        await PublishCurrentStateAsync(notification.NotificationId, notification.DomainEvent.OccuredOn, notification.DomainEvent.PluginId);
    }

    public async Task HandleAsync(PluginEnabledNotification notification)
    {
        await PublishCurrentStateAsync(notification.NotificationId, notification.DomainEvent.OccuredOn, notification.DomainEvent.PluginId);
    }

    public async Task HandleAsync(PluginDisabledNotification notification)
    {
        await PublishCurrentStateAsync(notification.NotificationId, notification.DomainEvent.OccuredOn, notification.DomainEvent.PluginId);
    }

    public async Task HandleAsync(PluginFunctionEnabledNotification notification)
    {
        await PublishCurrentStateAsync(notification.NotificationId, notification.DomainEvent.OccuredOn, notification.DomainEvent.PluginId);
    }

    public async Task HandleAsync(PluginFunctionDisabledNotification notification)
    {
        await PublishCurrentStateAsync(notification.NotificationId, notification.DomainEvent.OccuredOn, notification.DomainEvent.PluginId);
    }

    // the plugin and its configuration are gone, the last version says that no function is left
    public async Task HandleAsync(PluginConfigurationRemovedNotification notification)
    {
        var domainEvent = notification.DomainEvent;

        await _eventBus.PublishAsync(new PluginFunctionsChangedIntegrationEvent(
            notification.NotificationId,
            domainEvent.OccuredOn,
            domainEvent.PluginOwnerId.Value,
            domainEvent.PluginId.Value,
            domainEvent.Version,
            []));
    }

    private async Task PublishCurrentStateAsync(Guid notificationId, DateTime occurredOn, PluginId pluginId)
    {
        var configuration = await _readModel.Query<PluginConfiguration>()
            .Include(c => c.FunctionConfigurations)
            .FirstOrDefaultAsync(c => c.PluginId == pluginId);

        // removed after this change, the removal publishes the last state
        if (configuration is null)
            return;

        var plugin = await _readModel.Query<Plugin>()
            .Include(p => p.Functions)
            .FirstAsync(p => p.Id == pluginId);

        var availableIds = configuration.GetAvailableFunctionIds();
        var functions = plugin.Functions
            .Where(f => availableIds.Contains(f.Id))
            .Select(f => new AvailableFunctionSnapshot(f.Id.Value, PluginManifestMapper.ToContract(f.GetDescription())))
            .ToList();

        await _eventBus.PublishAsync(new PluginFunctionsChangedIntegrationEvent(
            notificationId,
            occurredOn,
            configuration.OwnerId.Value,
            pluginId.Value,
            configuration.Version,
            functions));
    }
}
