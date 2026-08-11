using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Types;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Visitors;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
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
    
    [ObsoleteMethodOnRepeatedType(typeof(IOutboxContext))]
    public ITypeKey<IOutboxContext> OutboxContext { get; set; }
    
    [ObsoleteMethodOnRepeatedType(typeof(IOutboxRepository))]
    public ITypeKey<IOutboxRepository> OutboxRepository { get; set; }
    
    [ObsoleteMethodOnRepeatedType(typeof(IOutboxProcessor))]
    public ITypeKey<IOutboxProcessor> OutboxProcessor { get; set; }
    
    [ObsoleteMethodOnRepeatedType(typeof(IInboxContext))]
    public ITypeKey<IInboxContext> InboxContext { get; set; }

    [ObsoleteMethodOnRepeatedType(typeof(IInboxRepository))]
    public ITypeKey<IInboxRepository> InboxRepository { get; set; }
    
    [ObsoleteMethodOnRepeatedType(typeof(IInboxProcessor))]
    public ITypeKey<IInboxProcessor> InboxProcessor { get; set; }
    

    
    public void Accept(IVisitor<ModuleMessagingConfiguration> visitor)
    {
        visitor.Visit(this);
    }
}


