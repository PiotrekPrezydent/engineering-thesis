using Dara.Server.BuildingBlocks.Infrastructure.Common.Types;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Visitors;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Persistence;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Processing;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Persistence;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Processing;
using Dara.Shared.Attributes;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration.Messaging;

[GenerateBuilder]
public partial class ModuleMessagingConfiguration : IVisitable<ModuleMessagingConfiguration>
{
    public Type DomainNotificationOpenGenericType { get; set; }
    
    [ObsoleteMethodOnRepeatedType(typeof(IOutboxRepository))]
    public ITypeKey<IOutboxRepository> OutboxRepository { get; set; }
    
    [ObsoleteMethodOnRepeatedType(typeof(IOutboxMessageProcessor))]
    public ITypeKey<IOutboxMessageProcessor> OutboxProcessor { get; set; }

    [ObsoleteMethodOnRepeatedType(typeof(IInboxRepository))]
    public ITypeKey<IInboxRepository> InboxRepository { get; set; }
    
    [ObsoleteMethodOnRepeatedType(typeof(IInboxMessageProcessor))]
    public ITypeKey<IInboxMessageProcessor> InboxProcessor { get; set; }
    

    
    public void Accept(IVisitor<ModuleMessagingConfiguration> visitor)
    {
        visitor.Visit(this);
    }
}


