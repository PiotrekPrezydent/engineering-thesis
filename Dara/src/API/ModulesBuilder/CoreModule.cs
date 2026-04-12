using Dara.Core.Domain.Commands;
using Dara.Core.Domain.Events;
using Dara.Core.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.API.ModulesBuilder;

public class CoreModule
{
    public CoreModule()
    {
        
    }
    
    public void AddToServices(IServiceCollection currentServices)
    {
        currentServices.AddScoped<IApplicationCommandDispatcher, ApplicationCommandDispatcher>();
        currentServices.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
    }
}