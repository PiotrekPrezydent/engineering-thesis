using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Types;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Visitors;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.References;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Mapping;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Persistence;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Processing;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Mapping;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Persistence;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Processing;
using Dara.Server.BuildingBlocks.Integration;
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
        _services.AddTransient(typeof(IOutboxProcessor),instance.OutboxProcessor.Value);
        _services.AddTransient(typeof(IInboxProcessor),instance.InboxProcessor.Value);
        
        _services.AddScoped(typeof(IOutboxRepository),instance.OutboxRepository.Value);
        _services.AddScoped(typeof(IInboxRepository),instance.InboxRepository.Value);
        _services.AddScoped<IEventBus>(_=>instance.EventBusInstance);

        var domainEventNotificationHandlers =
            _referencesConfiguration.ApplicationAssembly.GetImplementationsOfOpenGeneric(
                typeof(IDomainEventNotificationHandler<>)).ToList();
        
        var outboxMap = new Dictionary<string, Type>();
        
        foreach (var handler in domainEventNotificationHandlers)
        {
            var argument = handler.Interface.GenericTypeArguments[0];
            outboxMap.TryAdd(argument.Name, argument);
            
            _services.AddTransient(handler.Interface, handler.Implementation);
        }
        _services.AddSingleton<IOutboxTypeMapper>(_=>new OutboxTypeMapper(outboxMap));
        
        var integrationEventHandlers =
            _referencesConfiguration.ApplicationAssembly.GetImplementationsOfOpenGeneric(
                typeof(IIntegrationEventHandler<>)).ToList();
        
        var inboxMap = new Dictionary<string, Type>();
        
        foreach (var handler in integrationEventHandlers)
        {
            var argument = handler.Interface.GenericTypeArguments[0];
            inboxMap.TryAdd(argument.Name, argument);
            _services.AddTransient(handler.Interface, handler.Implementation);
            
            var eventTypeKey = Activator.CreateInstance(typeof(TypeKey<>).MakeGenericType(argument)) as ITypeKey<IIntegrationEvent>;
            eventTypeKey!.ExecuteGenericAction(new SubscribeEvent(instance.EventBusInstance,_referencesConfiguration.CompositionRoot));
        }
        _services.AddSingleton<IInboxTypeMapper>(_=>new InboxTypeMapper(inboxMap));

        _services.AddSingleton<OutboxQueueSignal>();
        _services.AddSingleton<InboxQueueSignal>();
        _services.AddSingleton<OutboxBackgroundWorker>();
        _services.AddSingleton<InboxBackgroundWorker>();
    }

    public class SubscribeEvent(IEventBus eventBus, IModuleCompositionRoot compositionRoot) : IKeyedTypeAction<IIntegrationEvent>
    {
        public void Execute<TType>(ITypeKey<IIntegrationEvent> typeKey) where TType : IIntegrationEvent
        {
            var handler = new InboxWriterIntegrationEventHandler<TType>(compositionRoot);
            eventBus.Subscribe(handler);
        }
    }
}