using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.BuildingBlocks.Domain.Events;
using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.BuildingBlocks.Infrastructure.Mediation.HandlerResolving;

public interface IHandlersResolver
{
    public ICommandHandler<TCommand> GetCommandHandler<TCommand>() 
        where TCommand : ICommand;
    
    public ICommandHandler<TCommand, TResult> GetCommandHandler<TCommand, TResult>() 
        where TCommand : ICommand<TResult>;

    public IEnumerable<IDomainEventHandler<TDomainEvent>> GetDomainEventHandlers<TDomainEvent>()
        where TDomainEvent : IDomainEvent;
    
    public IEnumerable<IDomainEventNotificationHandler<TDomainEventNotification>> GetDomainEventNotificationHandlers<TDomainEventNotification>() 
        where TDomainEventNotification : IDomainEventNotification;
    
    public IEnumerable<IIntegrationEventHandler<TIntegrationEvent>> GetIntegrationEventHandlers<TIntegrationEvent>() 
        where TIntegrationEvent : IIntegrationEvent;
}