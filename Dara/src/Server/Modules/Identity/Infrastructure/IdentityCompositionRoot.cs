using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Processing;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.References;
using Dara.Server.BuildingBlocks.Infrastructure.Extensions;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Commands;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Integration;
using Microsoft.Extensions.Logging;

namespace Dara.Server.Modules.Identity.Infrastructure;

public class IdentityCompositionRoot : ModuleCompositionRootBase
{
    protected override void ConfigureLogging(ILoggingBuilder loggingBuilder)
    {
        loggingBuilder.AddConsole();
    }

    protected override void ConfigureDataAccess(ModuleDataAccessDescriptor.ModuleDataAccessDescriptorBuilder builder)
    {
        builder
            .WithModuleContext(IdentityContext.AsTypeKey);
    }

    protected override void ConfigureReferences(ModuleReferencesDescriptor.ModuleReferencesDescriptorBuilder builder)
    {
        builder
            .WithApplicationAssembly(IIdentityModule.ContainingAssembly)
            .WithInfrastructureAssembly(IdentityModule.ContainingAssembly)
            .WithDeclaredModuleInterface(IIdentityModule.AsTypeKey)
            .WithOutboxConsumerType(typeof(IDomainEventNotificationHandler<>))
            .ConfigureMediationOpenTypes(e => e
                .AddRange(StandardMediationOpenTypes))
            .ConfigureTypeWiseDecorators(e => e
                .Add(typeof(CommandHandlerUnitOfWorkDecorator<>))
                .Add(typeof(CommandHandlerUnitOfWorkDecorator<,>)));
    }

    protected override void ConfigureProcessing(ModuleProcessingDescriptor.ModuleProcessingDescriptorBuilder builder)
    {
        builder
            .WithCommandExecutor(StandardCommandExecutor)
            .WithDomainEventDispatcher(StandardDomainEventsDispatcher)
            .WithHandlersResolver(StandardHandlersResolver)
            .WithUnitOfWork(StandardUnitOfWork);
    }

    protected override void ConfigureEvents(ModuleEventsDescriptor.ModuleEventsDescriptorBuilder builder)
    {
        builder
            .WithOutboxProcessor(StandardOutboxProcessor)
            .WithInboxProcessor(StandardInboxProcessor);
    }
}