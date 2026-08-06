using Dara.Server.BuildingBlocks.Infrastructure.Common.Visitors;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Commands;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.DomainEvents;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Persistence;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Scopes;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration.Processing;

public class ModuleProcessingVisitor : IVisitor<ModuleProcessingDescriptor>
{
    private readonly IServiceCollection _serviceCollection;

    public ModuleProcessingVisitor(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }
    
    public void Visit(ModuleProcessingDescriptor instance)
    {
        _serviceCollection.AddScoped(typeof(IDomainEventsDispatcher), instance.DomainEventDispatcher.Value);
        _serviceCollection.AddScoped(typeof(ICommandExecutor),  instance.CommandExecutor.Value);
        _serviceCollection.AddScoped(typeof(IHandlersResolver), instance.HandlersResolver.Value);
        _serviceCollection.AddScoped(typeof(IUnitOfWork), instance.UnitOfWork.Value);
    }
}