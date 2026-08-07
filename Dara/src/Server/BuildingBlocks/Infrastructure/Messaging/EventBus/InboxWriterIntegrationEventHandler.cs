using System.Text.Json;
using Dara.Server.BuildingBlocks.Application;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox;
using Dara.Server.BuildingBlocks.Integration;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;

public class InboxWriterIntegrationEventHandler<TIntegrationEvent> : IIntegrationEventHandler<TIntegrationEvent> where TIntegrationEvent : IIntegrationEvent
{
    readonly IModuleCompositionRoot  _compositionRoot;
    
    public InboxWriterIntegrationEventHandler(IModuleCompositionRoot compositionRoot)
    {
        _compositionRoot = compositionRoot;
    }
    
    public async Task HandleAsync(TIntegrationEvent integrationEvent)
    {
        using var scope = _compositionRoot.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ModuleContext>();
        var data = JsonSerializer.Serialize(integrationEvent,integrationEvent.GetType());
        
        var message = new InboxMessage(
            Guid.NewGuid(),
            DateTime.Now,
            integrationEvent.GetType().Name,
            data
            );

        await context.InboxMessages.AddAsync(message);
        await context.SaveChangesAsync();
    }
}