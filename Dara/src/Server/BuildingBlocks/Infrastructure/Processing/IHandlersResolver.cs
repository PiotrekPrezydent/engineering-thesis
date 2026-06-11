using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.BuildingBlocks.Infrastructure.Processing;

public interface IHandlersResolver
{
    public ICommandHandler<TCommand> GetCommandHandler<TCommand>() 
        where TCommand : ICommand;
    
    public ICommandHandler<TCommand, TResult> GetCommandHandler<TCommand, TResult>() 
        where TCommand : ICommand<TResult>;

    public IEnumerable<IDomainEventHandler<TDomainEvent>> GetDomainEventHandlers<TDomainEvent>()
        where TDomainEvent : IDomainEvent;
}