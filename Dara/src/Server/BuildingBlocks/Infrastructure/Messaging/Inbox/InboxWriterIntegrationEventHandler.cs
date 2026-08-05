using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox;

public class InboxWriterIntegrationEventHandler<TIntegrationEvent> : IIntegrationEventHandler<TIntegrationEvent> where TIntegrationEvent : IIntegrationEvent
{
    readonly ModuleContext _moduleContext;
    
    public InboxWriterIntegrationEventHandler(ModuleContext moduleContext)
    {
        _moduleContext = moduleContext;
    }
    
    public async Task HandleAsync(TIntegrationEvent integrationEvent)
    {
        var message = new InboxMessage(
            Guid.NewGuid(),
            DateTime.Now,
            integrationEvent.GetType().Name,
            "WIP"
            );

        await _moduleContext.InboxMessages.AddAsync(message);
    }
}