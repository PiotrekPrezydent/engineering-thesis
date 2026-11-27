using System.Reflection;
using System.Threading.Channels;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Types;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Apps.API.Common.Notifications;

// every IHubNotificationHandler of the assembly subscribes its event on the bus; the events go through one channel
// to HubNotificationsProcessor, which calls the handler in its own scope
public static class HubNotificationsSetup
{
    public static void AddHubNotifications(this IServiceCollection services, Assembly assembly, IEventBus eventBus)
    {
        var channel = Channel.CreateUnbounded<IIntegrationEvent>();
        services.AddSingleton(channel);
        services.AddSingleton(eventBus);

        var handlers = assembly.GetImplementationsOfOpenGeneric(typeof(IHubNotificationHandler<>));

        foreach (var handler in handlers)
        {
            var eventType = handler.Interface.GenericTypeArguments[0];
            var eventTypeKey = Activator.CreateInstance(typeof(TypeKey<>).MakeGenericType(eventType)) as ITypeKey<IIntegrationEvent>;

            services.AddScoped(handler.Interface, handler.Implementation);

            eventTypeKey!.ExecuteGenericAction(new ChannelWriterIntegrationEventRegistrator(channel, eventBus));
        }

        services.AddHostedService<HubNotificationsProcessor>();
    }

    private class ChannelWriterIntegrationEventRegistrator(Channel<IIntegrationEvent> channel, IEventBus eventBus) : ITypeKeyAction<IIntegrationEvent>
    {
        public void Execute<TType>(ITypeKey<IIntegrationEvent> typeKey) where TType : IIntegrationEvent
        {
            var handler = new ChannelWriterIntegrationEventHandler<TType>(channel);
            eventBus.Subscribe(handler);
        }
    }
}
