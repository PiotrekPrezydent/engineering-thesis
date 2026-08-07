using Dara.Server.BuildingBlocks.Infrastructure.Common.Types;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Visitors;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox;
using Dara.Server.BuildingBlocks.Integration;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration.Events;

public class ModuleEventsVisitor : IVisitor<ModuleEventsDescriptor>
{
    readonly IServiceCollection _services;
    private readonly IModuleCompositionRoot _context;

    public ModuleEventsVisitor(IServiceCollection serviceCollection, IModuleCompositionRoot context)
    {
        _services = serviceCollection;
        _context = context;
    }
    public void Visit(ModuleEventsDescriptor instance)
    {
        Console.WriteLine("VISIT");
        _services.AddTransient(typeof(IOutboxProcessor),instance.OutboxProcessor.Value);
        _services.AddTransient(typeof(IInboxProcessor),instance.InboxProcessor.Value);
        _services.AddSingleton<OutboxProcessorService>();
        _services.AddSingleton<InboxProcessorService>();
        _services.AddScoped<IEventBus>(_=>InMemoryEventBus.Instance);

        foreach (var integrationEvent in instance.Events)
        {
            integrationEvent.ExecuteGenericAction(new SubscribeEvent(_context));
        }
    }

    public class AddEventBus(IServiceCollection services) : IKeyedTypeAction<IEventBus>
    {
        public void Execute<TType>(ITypeKey<IEventBus> typeKey) where TType : IEventBus
        {
            //throw new NotImplementedException();
        }
    }

    public class SubscribeEvent(IModuleCompositionRoot compositionRoot) : IKeyedTypeAction<IIntegrationEvent>
    {
        public void Execute<TType>(ITypeKey<IIntegrationEvent> typeKey) where TType : IIntegrationEvent
        {
            var handler = new InboxWriterIntegrationEventHandler<TType>(compositionRoot);
            InMemoryEventBus.Instance.Subscribe(handler);
        }
    }
}