using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Mediation;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Messaging;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Processing;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.References;
using Dara.Server.Modules.Orchestration.Application;

namespace Dara.Server.Modules.Orchestration.Infrastructure;

public class OrchestrationCompositionRoot : ModuleCompositionRootBase
{
    protected override void ConfigureReferences(ModuleReferencesConfiguration.ModuleReferencesConfigurationBuilder builder)
    {
        builder
            .WithApplicationAssembly(IOrchestrationModule.ContainingAssembly)
            .WithInfrastructureAssembly(OrchestrationModule.ContainingAssembly)
            .WithDeclaredModuleInterface(IOrchestrationModule.AsTypeKey)
            .WithCompositionRoot(this);
    }

    protected override void ConfigureDataAccess(ModuleDataAccessConfiguration.ModuleDataAccessConfigurationBuilder builder)
    {
        AddStandardDataAccess<OrchestrationContext>(builder);
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
        AddStandardMessaging<OrchestrationContext>(builder);
    }
}