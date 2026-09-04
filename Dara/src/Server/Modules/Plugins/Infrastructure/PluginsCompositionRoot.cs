using System.Collections.Immutable;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Mediation;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Messaging;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Processing;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.References;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Plugins.Application;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Dara.Server.Modules.Plugins.Domain.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.Modules.Plugins.Infrastructure;

public class PluginsCompositionRoot : ModuleCompositionRootBase
{
    protected override void ConfigureReferences(ModuleReferencesConfiguration.ModuleReferencesConfigurationBuilder builder)
    {
        builder
            .WithApplicationAssembly(IPluginsModule.ContainingAssembly)
            .WithInfrastructureAssembly(PluginsModule.ContainingAssembly)
            .WithDeclaredModuleInterface(IPluginsModule.AsTypeKey)
            .WithCompositionRoot(this);
    }

    protected override void ConfigureDataAccess(ModuleDataAccessConfiguration.ModuleDataAccessConfigurationBuilder builder)
    {
        AddStandardDataAccess<PluginsContext>(builder);
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
        AddStandardMessaging<PluginsContext>(builder);
    }
}