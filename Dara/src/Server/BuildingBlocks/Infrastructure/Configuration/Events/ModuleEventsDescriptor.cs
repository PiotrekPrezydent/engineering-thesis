using Dara.Server.BuildingBlocks.Infrastructure.Common.Types;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Visitors;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox;
using Dara.Server.BuildingBlocks.Integration;
using Dara.Shared.Attributes;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration.Events;

[GenerateBuilder]
public partial class ModuleEventsDescriptor : IVisitable<ModuleEventsDescriptor>
{
    [ObsoleteMethodOnRepeatedType(typeof(IIntegrationEvent))]
    public IReadOnlyList<ITypeKey<IIntegrationEvent>> Events { get; set; }
    
    [ObsoleteMethodOnRepeatedType(typeof(IEventBus))]
    public ITypeKey<IEventBus> EventBus { get; set; }
    
    [ObsoleteMethodOnRepeatedType(typeof(IOutboxProcessor))]
    public ITypeKey<IOutboxProcessor> OutboxProcessor { get; set; }
    
    [ObsoleteMethodOnRepeatedType(typeof(IInboxProcessor))]
    public ITypeKey<IInboxProcessor> InboxProcessor { get; set; }
    
    public TimeSpan OutboxPollingInterval { get; set; }
    
    public void Accept(IVisitor<ModuleEventsDescriptor> visitor)
    {
        visitor.Visit(this);
    }
}


