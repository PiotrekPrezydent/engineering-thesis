using System.Text.Json;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Mapping;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Persistence;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Processing;
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
        
        var repository = scope.ServiceProvider.GetRequiredService<IInboxRepository>();
        var mapper = scope.ServiceProvider.GetRequiredService<IInboxMessagesTypeMapper>();
        var signal = scope.ServiceProvider.GetRequiredService<InboxQueueSignal>();

        
        var data = JsonSerializer.Serialize(integrationEvent,integrationEvent.GetType());
        var type = mapper.GetTypeNameForMessageWithType(integrationEvent.GetType());
        
        var message = new InboxMessage(
            integrationEvent.Id,
            integrationEvent.OccurredOn,
            type,
            data
            );
        
        await repository.SaveAsync(message, CancellationToken.None);
        signal.NotifyNewMessage();
    }
}