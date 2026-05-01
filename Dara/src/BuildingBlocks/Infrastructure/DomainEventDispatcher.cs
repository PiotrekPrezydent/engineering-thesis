using Dara.BuildingBlocks.Application;
using Dara.BuildingBlocks.Domain.Events;
using Dara.BuildingBlocks.Domain.Models;
using Dara.BuildingBlocks.Infrastructure.Abstractions;
using Dara.Shared.Common;
using Dara.Shared.Common.CLI;
using Dara.Shared.Common.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.BuildingBlocks.Infrastructure;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    readonly protected IServiceProvider _serviceProvider;
    protected Logger _logger;

    public DomainEventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = new(nameof(DomainEventDispatcher),this, LoggingType.Console);
    }

    public async Task<WrappedResult> DispatchSingleEventAsync<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
    {
        var handler = _serviceProvider.GetRequiredService<IHandler<TDomainEvent>>();
        WrappedResult wrappedResult;

        try
        {
            await handler.HandleAsync(domainEvent);
            wrappedResult = new();
        }
        catch(Exception ex)
        {
            wrappedResult = ex;
        }
        
        return wrappedResult;
    }

    public async Task DispatchEntityEventsAsync(Entity entity)
    {
        foreach (var domainEvent in entity.DomainEvents)
        {
           var check = await DispatchSingleEventAsync(domainEvent);
        }
        
        entity.ClearDomainEvents();
    }
}