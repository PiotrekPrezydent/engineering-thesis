using Dara.Server.BuildingBlocks.Infrastructure.Configuration.CompositionRoot;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.ModuleDescriptors;
using Dara.Server.BuildingBlocks.Infrastructure.Extensions;
using Dara.Server.Modules.Groups.Application;

namespace Dara.Server.Modules.Groups.Infrastructure;

public class GroupModuleCompositionRoot : ModuleCompositionRootBase
{
    protected override void ConfigureDataAccess(ModuleDataAccessDescriptor.ModuleDataAccessDescriptorBuilder builder)
    {
        builder.WithModuleContext(GroupModuleContext.ToTypeKey());
    }

    protected override void ConfigureReferences(ModuleReferencesDescriptor.ModuleReferencesDescriptorBuilder builder)
    {
        builder.WithApplicationAssembly(typeof(IGroupModule).Assembly);
        builder.WithInfrastructureAssembly(typeof(GroupModule).Assembly);
        builder.ConfigureMediationOpenTypes(e => e.AddRange(StandardMediationOpenTypes));
        builder.WithDeclaredModuleInterface(IGroupModule.ToTypeKey());
    }

    protected override void ConfigureProcessing(ModuleProcessingDescriptor.ModuleProcessingDescriptorBuilder builder)
    {
        builder.WithCommandExecutor(StandardCommandExecutor);
        builder.WithDomainEventDispatcher(StandardDomainEventsDispatcher);
        builder.WithHandlersResolver(StandardHandlersResolver);
        builder.WithUnitOfWork(StandardUnitOfWork);
    }

    protected override void ConfigureEvents(ModuleEventsDescriptor.ModuleEventsDescriptorBuilder builder)
    {
        
        //throw new NotImplementedException();
    }
}