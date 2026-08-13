using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;
using Dara.Server.Modules.Plugins.Integration;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Plugins.Application.AddPlugin;

public class PublishPluginAddedNotificationHandler : IDomainEventNotificationHandler<PluginAddedNotification>
{
    private readonly IEventBus _eventBus;
    private readonly IReadModel _readModel;

    public PublishPluginAddedNotificationHandler(IEventBus eventBus, IReadModel readModel)
    {
        _eventBus = eventBus;
        _readModel = readModel;
    }

    public async Task HandleAsync(PluginAddedNotification notification)
    {
        var owner = await _readModel.Query<PluginOwner>()
            .Include(p=>p.Plugins)
            .ThenInclude(p => p.Functions)
            .FirstAsync(e => e.Id.Match(notification.DomainEvent.PluginOwnerId));
        
        var plugin = owner.Plugins.Single(e=>e.Id.Match(notification.DomainEvent.PluginId));

        var snapshot = new PluginSnapshot(plugin.Id,
            plugin.Name,
            plugin.Description,
            plugin.Functions.Select(func => new PluginFunctionSnapshot(func.Id,
                    func.Name,
                    func.Description,
                    func.ReturnType,
                    func.Parameters.Select(param => new PluginFunctionParameterSnapshot(param.Name,
                            param.Type,
                            param.Description))
                        .ToList()))
                .ToList());
            
        
        await _eventBus.PublishAsync(new PluginAddedIntegrationEvent(
            notification.NotificationId,
            notification.DomainEvent.OccuredOn,
            notification.DomainEvent.PluginOwnerId,
            snapshot
        ));
    }
}