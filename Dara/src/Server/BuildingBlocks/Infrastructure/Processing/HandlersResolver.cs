using Dara.BuildingBlocks.Domain.Events;
using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Integration;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.BuildingBlocks.Infrastructure.Processing;

public class HandlersResolver : IHandlersResolver
{
    readonly IServiceProvider _serviceProvider;

    public HandlersResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ICommandHandler<TCommand> GetCommandHandler<TCommand>() 
        where TCommand : ICommand
    {
        return _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
    }

    public ICommandHandler<TCommand, TResult> GetCommandHandler<TCommand, TResult>() 
        where TCommand : ICommand<TResult>
    {
        return _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
    }

    public IEnumerable<IDomainEventHandler<TDomainEvent>> GetDomainEventHandlers<TDomainEvent>() 
        where TDomainEvent : IDomainEvent
    {
        return _serviceProvider.GetServices<IDomainEventHandler<TDomainEvent>>();
    }

    public IEnumerable<IIntegrationEventHandler<TIntegrationEvent>> GetIntegrationEventHandlers<TIntegrationEvent>() 
        where TIntegrationEvent : IIntegrationEvent
    {
        return _serviceProvider.GetServices<IIntegrationEventHandler<TIntegrationEvent>>();
    }
}