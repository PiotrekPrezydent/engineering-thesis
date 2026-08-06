using Dara.Server.BuildingBlocks.Infrastructure.Common.Types;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Visitors;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.BuildingBlocks.Integration;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration.Events;

public class ModuleEventsVisitor : IVisitor<ModuleEventsDescriptor>
{
    readonly IServiceCollection _services;
    public ModuleEventsVisitor(IServiceCollection serviceCollection)
    {
        _services = serviceCollection;
    }
    public void Visit(ModuleEventsDescriptor instance)
    {
    }

    public class AddEventBus(IServiceCollection services) : IKeyedTypeAction<IEventBus>
    {
        public void Execute<TType>(ITypeKey<IEventBus> typeKey) where TType : IEventBus
        {
            //throw new NotImplementedException();
        }
    }

    public class SubscribeEvent : IKeyedTypeAction<IIntegrationEvent>
    {
        public void Execute<TType>(ITypeKey<IIntegrationEvent> typeKey) where TType : IIntegrationEvent
        {
            //throw new NotImplementedException();
        }
    }
}