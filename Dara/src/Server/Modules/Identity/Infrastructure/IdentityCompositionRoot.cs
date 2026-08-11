using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Processing;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.References;
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
        AddStandardDataAccess<IdentityContext>(builder);
    }

    protected override void ConfigureMediation(ModuleMediationConfiguration.ModuleMediationConfigurationBuilder builder)
    {
        AddStandardMediation(builder);
    }

    protected override void ConfigureProcessing(ModuleProcessingConfiguration.ModuleProcessingConfigurationBuilder builder)
    {
        AddStandardProcessing(builder);
    }

    protected override void ConfigureMessaging(ModuleMessagingConfiguration.ModuleMessagingConfigurationBuilder builder)
    {
        AddStandardMessaging<IdentityContext>(builder);
    }
}