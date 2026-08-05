using Dara.Server.BuildingBlocks.Infrastructure.Common.Types;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.ModuleDescriptors;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Visitors;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration.CompositionRoot;



public abstract class ModuleCompositionRootBase : IModuleCompositionRoot
{
    private IServiceProvider _services;

    protected ModuleCompositionRootBase() { }
    
    public IServiceScope CreateScope()
    {
        return _services.CreateScope();
    }
    
    public void Initialize(IServiceCollection rootServices)
    {
        IServiceCollection services = new ServiceCollection();
        
        ModuleDataAccessDescriptor.ModuleDataAccessDescriptorBuilder dataAccessBuilder = new();
        ModuleReferencesDescriptor.ModuleReferencesDescriptorBuilder refencesBuilder = new();
        ModuleProcessingDescriptor.ModuleProcessingDescriptorBuilder processingBuilder = new();
        ModuleEventsDescriptor.ModuleEventsDescriptorBuilder eventsBuilder = new();
        
        ConfigureDataAccess(dataAccessBuilder);
        ConfigureRefernces(refencesBuilder);
        ConfigureProcessing(processingBuilder);
        ConfigureEvents(eventsBuilder);
        
        var dataAccess = dataAccessBuilder.Build();
        var references = refencesBuilder.Build();
        var processing = processingBuilder.Build();
        var events = eventsBuilder.Build();
        
        ModuleDataAccessVisitor dataAccessVisitor = new(services);
        ModuleReferencesVisitor referencesVisitor = new();
        ModuleProcessingVisitor processingVisitor = new();
        ModuleEventsVisitor eventsVisitor = new(services);
        
        dataAccess.Accept(dataAccessVisitor);
        references.Accept(referencesVisitor);
        processing.Accept(processingVisitor);
        events.Accept(eventsVisitor);
    }
    
    protected abstract void ConfigureDataAccess(ModuleDataAccessDescriptor.ModuleDataAccessDescriptorBuilder builder);
    
    protected abstract void ConfigureRefernces(ModuleReferencesDescriptor.ModuleReferencesDescriptorBuilder builder);
    
    protected abstract void ConfigureProcessing(ModuleProcessingDescriptor.ModuleProcessingDescriptorBuilder builder);
    
    protected abstract void ConfigureEvents(ModuleEventsDescriptor.ModuleEventsDescriptorBuilder builder);
}



