using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Visitors;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.References;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Commands;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.DomainEvents;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.InternalCommands;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration.Processing;

public class ModuleProcessingVisitor : IVisitor<ModuleProcessingConfiguration>
{
    private readonly ModuleReferencesConfiguration _referencesConfiguration;
    private readonly IServiceCollection _serviceCollection;

    public ModuleProcessingVisitor(ModuleReferencesConfiguration referencesConfiguration, IServiceCollection serviceCollection)
    {
        _referencesConfiguration = referencesConfiguration;
        _serviceCollection = serviceCollection;
    }

    public void Visit(ModuleProcessingConfiguration instance)
    {
        _serviceCollection.AddScoped(typeof(IDomainEventsDispatcher), instance.DomainEventDispatcher.Value);
        _serviceCollection.AddScoped(typeof(ICommandExecutor),  instance.CommandExecutor.Value);

        _serviceCollection.AddScoped(typeof(IInternalCommandScheduler), instance.InternalCommandScheduler.Value);
        _serviceCollection.AddScoped(typeof(IInternalCommandProcessor), instance.InternalCommandProcessor.Value);
        _serviceCollection.AddScoped<IInternalCommandRepository, InternalCommandRepository>();
        _serviceCollection.AddSingleton<IInternalCommandsTypeMapper>(InternalCommandsTypeMapper.FromAssembly(_referencesConfiguration.ApplicationAssembly));
        _serviceCollection.AddSingleton<InternalCommandQueueSignal>();
        _serviceCollection.AddSingleton<InternalCommandsBackgroundWorker>();
    }
}
