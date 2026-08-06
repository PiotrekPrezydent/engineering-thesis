using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.Common;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Types;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Processing;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.References;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Commands;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.DomainEvents;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Persistence;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Scopes;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration;

public abstract class ModuleCompositionRootBase : IModuleCompositionRoot
{
    private IServiceProvider _services = null!;
    
    public IServiceScope CreateScope()
    { 
        if(_services == null)
            throw  new InvalidOperationException($"{GetType().FullName} is not initialized.");
        
        return _services.CreateScope();
    }

    private void SetServiceProvider(IServiceProvider serviceProvider)
    {
        _services = serviceProvider;
    }
    

    public void Initialize(IServiceCollection rootServices)
    {
        IServiceCollection services = new ServiceCollection();
        
        ModuleDataAccessDescriptor.ModuleDataAccessDescriptorBuilder dataAccessBuilder = new();
        ModuleReferencesDescriptor.ModuleReferencesDescriptorBuilder refencesBuilder = new();
        ModuleProcessingDescriptor.ModuleProcessingDescriptorBuilder processingBuilder = new();
        ModuleEventsDescriptor.ModuleEventsDescriptorBuilder eventsBuilder = new();
        
        ConfigureDataAccess(dataAccessBuilder);
        ConfigureReferences(refencesBuilder);
        ConfigureProcessing(processingBuilder);
        ConfigureEvents(eventsBuilder);
        
        var dataAccess = dataAccessBuilder.Build();
        var references = refencesBuilder.Build();
        var processing = processingBuilder.Build();
        var events = eventsBuilder.Build();
        
        ModuleReferencesVisitor referencesVisitor = new(services);
        ModuleDataAccessVisitor dataAccessVisitor = new(services);
        ModuleProcessingVisitor processingVisitor = new(services);
        ModuleEventsVisitor eventsVisitor = new(services);
        
        dataAccess.Accept(dataAccessVisitor);
        references.Accept(referencesVisitor);
        processing.Accept(processingVisitor);
        events.Accept(eventsVisitor);

        services.AddSingleton<IModuleCompositionRoot>(this);
        SetServiceProvider(services.BuildServiceProvider());
        
        var moduleInterface = references.DeclaredModuleInterface.Value;
        
        rootServices.AddScoped(moduleInterface, _ => _services.GetRequiredService(moduleInterface));
    }
    
    protected abstract void ConfigureDataAccess(ModuleDataAccessDescriptor.ModuleDataAccessDescriptorBuilder builder);
    
    protected abstract void ConfigureReferences(ModuleReferencesDescriptor.ModuleReferencesDescriptorBuilder builder);
    
    protected abstract void ConfigureProcessing(ModuleProcessingDescriptor.ModuleProcessingDescriptorBuilder builder);
    
    protected abstract void ConfigureEvents(ModuleEventsDescriptor.ModuleEventsDescriptorBuilder builder);

    protected static IReadOnlyList<Type> StandardMediationOpenTypes => new List<Type>
    {
        typeof(ICommandHandler<>),
        typeof(ICommandHandler<,>),
        typeof(IQueryHandler<,>),
        typeof(IDomainEventHandler<>)
    };
    
    protected static ITypeKey<DomainEventsDispatcher> StandardDomainEventsDispatcher => new TypeKey<DomainEventsDispatcher>();
    protected static ITypeKey<CommandExecutor> StandardCommandExecutor => new TypeKey<CommandExecutor>();
    protected static ITypeKey<HandlersResolver> StandardHandlersResolver => new TypeKey<HandlersResolver>();
    protected static ITypeKey<UnitOfWork> StandardUnitOfWork => new TypeKey<UnitOfWork>();
}



