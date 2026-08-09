using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Visitors;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Mediation.HandlerResolving;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Commands;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.DomainEvents;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration.Processing;

public class ModuleProcessingVisitor : IVisitor<ModuleProcessingConfiguration>
{
    private readonly IServiceCollection _serviceCollection;

    public ModuleProcessingVisitor(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }
    
    public void Visit(ModuleProcessingConfiguration instance)
    {
        _serviceCollection.AddScoped(typeof(IDomainEventsDispatcher), instance.DomainEventDispatcher.Value);
        _serviceCollection.AddScoped(typeof(ICommandExecutor),  instance.CommandExecutor.Value);
        _serviceCollection.AddScoped(typeof(IInternalCommandExecutor), instance.CommandExecutor.Value);
    }
}