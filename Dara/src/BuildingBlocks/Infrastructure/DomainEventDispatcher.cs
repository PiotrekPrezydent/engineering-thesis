using Dara.BuildingBlocks.Application;
using Dara.BuildingBlocks.Domain.Events;
using Dara.BuildingBlocks.Domain.Models;
using Dara.BuildingBlocks.Infrastructure.Abstractions;
using Dara.Shared.Common;
using Dara.Shared.Common.CLI;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.BuildingBlocks.Infrastructure;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    readonly protected IServiceProvider _serviceProvider;
    protected CLILogger _logger;

    public DomainEventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = new(this.GetType().Name, ConsoleColor.Green);
    }

    public async Task<WrappedResult> DispatchSingleEventAsync<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
    {
        var handler = _serviceProvider.GetRequiredService<IHandler<TDomainEvent>>();
        WrappedResult wrappedResult;
        
        string log1 = $"Calling handler of type: {handler.GetType().Name} for request of type {domainEvent.GetType().Name}";
        string log2 = $"Request value: {domainEvent}";
        string log3;

        try
        {
            await handler.HandleAsync(domainEvent);
            
            log3 = $"Handled without exception.";
            wrappedResult = new();
        }
        catch(Exception ex)
        {
            log3 = $"Handled with exception of type: {ex.GetType().Name}.";
            wrappedResult = ex;
        }
        
        _logger.LogMany(log1, log2, log3);
        
        return wrappedResult;
    }

    public async Task DispatchEntityEventsAsync(Entity entity)
    {
        foreach (var domainEvent in entity.DomainEvents)
        {
           var check = await DispatchSingleEventAsync(domainEvent);
           
           if (!check.IsSuccess)
               _logger.Log(check.Error!.Message);
        }
        
        entity.ClearDomainEvents();
    }
}