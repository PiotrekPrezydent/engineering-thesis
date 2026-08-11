using Dara.Server.BuildingBlocks.Infrastructure.Common;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Visitors;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.References;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess.DomainEventsMapping;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Persistence;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Processing;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Mapping;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Persistence;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Processing;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration.Messaging;

public class ModuleMessagingVisitor : IVisitor<ModuleMessagingConfiguration>
{
    private readonly ModuleReferencesConfiguration _referencesConfiguration;
    readonly IServiceCollection _services;

    public ModuleMessagingVisitor(ModuleReferencesConfiguration referencesConfiguration, IServiceCollection serviceCollection)
    {
        _referencesConfiguration = referencesConfiguration;
        _services = serviceCollection;
    }
    
    public void Visit(ModuleMessagingConfiguration instance)
    {
        _services.AddScoped(typeof(IOutboxContext),instance.OutboxContext.Value);
        _services.AddScoped(typeof(IOutboxRepository),instance.OutboxRepository.Value);
        _services.AddTransient(typeof(IOutboxProcessor),instance.OutboxProcessor.Value);
        
        _services.AddScoped(typeof(IInboxContext),instance.InboxContext.Value);
        _services.AddScoped(typeof(IInboxRepository),instance.InboxRepository.Value);
        _services.AddTransient(typeof(IInboxProcessor),instance.InboxProcessor.Value);
        
        var notifications = _referencesConfiguration.ApplicationAssembly
            .GetImplementationsOfOpenGeneric(instance.DomainNotificationOpenGenericType).ToList();

        BiDictionary<Type, string> outboxMap = new();
        Dictionary<Type, Type> notificationsMap = new();
        
        foreach (var notification in notifications)
        {
            var domainEventType = notification.Interface.GenericTypeArguments[0];
            
            outboxMap.Add(notification.Implementation,notification.Implementation.Name);
            notificationsMap.Add(domainEventType, notification.Implementation);
        }
        
        _services.AddSingleton<IOutboxMessagesTypeMapper>(new OutboxMessagesTypeMapper(outboxMap));
        _services.AddSingleton<IDomainEventNotificationMapper>(new DomainEventNotificationMapper(notificationsMap));

        _services.AddSingleton<OutboxQueueSignal>();
        _services.AddSingleton<InboxQueueSignal>();
        _services.AddSingleton<OutboxBackgroundWorker>();
        _services.AddSingleton<InboxBackgroundWorker>();
    }
    
}