using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Processing;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.References;
using Dara.Server.BuildingBlocks.Infrastructure.Extensions;
using Dara.Server.Modules.Groups.Application;
using Microsoft.Extensions.Logging;

namespace Dara.Server.Modules.Groups.Infrastructure;

public class GroupCompositionRoot : ModuleCompositionRootBase
{
    protected override void ConfigureLogging(ILoggingBuilder loggingBuilder)
    {
        loggingBuilder.AddConsole();
    }

    protected override void ConfigureDataAccess(ModuleDataAccessDescriptor.ModuleDataAccessDescriptorBuilder builder)
    {
        builder.WithModuleContext(GroupContext.AsTypeKey);
    }

    protected override void ConfigureReferences(ModuleReferencesDescriptor.ModuleReferencesDescriptorBuilder builder)
    {
        builder.WithApplicationAssembly(typeof(IGroupModule).Assembly);
        builder.WithInfrastructureAssembly(typeof(GroupModule).Assembly);
        builder.ConfigureMediationOpenTypes(e => e.AddRange(StandardMediationOpenTypes));
        builder.WithDeclaredModuleInterface(IGroupModule.AsTypeKey);
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
        builder.WithOutboxProcessor(StandardOutboxProcessor);   
    }
}