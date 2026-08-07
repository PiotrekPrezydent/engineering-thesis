using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Processing;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.References;
using Dara.Server.BuildingBlocks.Infrastructure.Mediation.Decorators;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Persistence;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Persistence;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Integration;
using Microsoft.Extensions.Logging;
using ModuleMediationConfiguration = Dara.Server.BuildingBlocks.Infrastructure.Configuration.Mediation.ModuleMediationConfiguration;
using ModuleMessagingConfiguration = Dara.Server.BuildingBlocks.Infrastructure.Configuration.Messaging.ModuleMessagingConfiguration;

namespace Dara.Server.Modules.Identity.Infrastructure;

public class IdentityCompositionRoot : ModuleCompositionRootBase
{
    protected override void ConfigureLogging(ILoggingBuilder loggingBuilder)
    {
        loggingBuilder
            .AddConsole();
    }

    protected override void ConfigureReferences(ModuleReferencesConfiguration.ModuleReferencesConfigurationBuilder builder)
    {
        builder
            .WithApplicationAssembly(IIdentityModule.ContainingAssembly)
            .WithInfrastructureAssembly(IdentityModule.ContainingAssembly)
            .WithDeclaredModuleInterface(IIdentityModule.AsTypeKey)
            .WithCompositionRoot(this);
    }

    protected override void ConfigureDataAccess(ModuleDataAccessConfiguration.ModuleDataAccessConfigurationBuilder builder)
    {
        builder
            .WithModuleContext(IdentityContext.AsTypeKey)
            .WithUnitOfWork(StandardUnitOfWork);
    }

    protected override void ConfigureMediation(ModuleMediationConfiguration.ModuleMediationConfigurationBuilder builder)
    {
        builder
            .ConfigureMediationOpenTypes(e => e
                .AddRange(StandardMediationOpenTypes))
            .ConfigureTypeWiseDecorators(e => e
                .Add(typeof(CommandHandlerUnitOfWorkDecorator<,>)))
            .WithHandlersResolver(StandardHandlersResolver);
    }

    protected override void ConfigureProcessing(ModuleProcessingConfiguration.ModuleProcessingConfigurationBuilder builder)
    {
        builder
            .WithCommandExecutor(StandardCommandExecutor)
            .WithDomainEventDispatcher(StandardDomainEventsDispatcher);
    }

    protected override void ConfigureMessaging(ModuleMessagingConfiguration.ModuleMessagingConfigurationBuilder builder)
    {
        builder
            .WithEventBusInstance(InMemoryEventBus.Instance)
            .WithInboxProcessor(StandardInboxProcessor)
            .WithOutboxProcessor(StandardOutboxProcessor)
            .WithInboxRepository(InboxRepository<IdentityContext>.AsTypeKey)
            .WithOutboxRepository(OutboxRepository<IdentityContext>.AsTypeKey);
    }
}