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
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;
using Dara.Server.Modules.Plugins.Infrastructure.PluginOwners;
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

    protected override void OnServiceProviderCreated(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<PluginsContext>();
        context.Database.EnsureCreated();

        context.PluginOwners.AddRange(
            SeedPluginOwner(SharedSeedGuids.User1, 
                ("AudioControl", "", PluginFunctionsSeeds.GetAudioControlFunctions()),
                ("PowerManagement","",PluginFunctionsSeeds.GetPowerManagementFunctions()),
                ("CrossDeviceHandoff","",PluginFunctionsSeeds.GetCrossDeviceHandoffFunctions()),
                ("VoiceInteraction","",PluginFunctionsSeeds.GetVoiceInteractionFunctions())
            ),
            SeedPluginOwner(SharedSeedGuids.User2, 
                ("Telephony", "", PluginFunctionsSeeds.GetTelephonyFunctions()),
                ("DeviceLocator", "", PluginFunctionsSeeds.GetDeviceLocatorFunctions()),
                ("RemoteSensor","",PluginFunctionsSeeds.GetRemoteSensorFunctions()),
                ("RemoteAuthentication","",PluginFunctionsSeeds.GetRemoteAuthenticationFunctions()),
                ("NotificationSync","",PluginFunctionsSeeds.GetNotificationSyncFunctions()),
                ("CrossDeviceHandoff","",PluginFunctionsSeeds.GetCrossDeviceHandoffFunctions())
            ),
            SeedPluginOwner(SharedSeedGuids.User3, 
                ("AudioControl", "", PluginFunctionsSeeds.GetAudioControlFunctions()),
                ("VoiceInteraction","",PluginFunctionsSeeds.GetVoiceInteractionFunctions()),
                ("RemoteSensor","",PluginFunctionsSeeds.GetRemoteSensorFunctions())
            ),
            SeedPluginOwner(SharedSeedGuids.User4, 
                ("PowerManagement", "", PluginFunctionsSeeds.GetPowerManagementFunctions())
            ),
            SeedPluginOwner(SharedSeedGuids.User5, 
                ("AudioControl", "",PluginFunctionsSeeds.GetAudioControlFunctions()), 
                ("CrossDeviceHandoff", "", PluginFunctionsSeeds.GetCrossDeviceHandoffFunctions())
            )
        );
        
        context.SaveChanges();
    }
    
    PluginOwner SeedPluginOwner(Guid ownerId, params (string name, string description, ImmutableArray<PluginFunction> functions)[] plugins)
    {
        var owner = PluginOwner.Create(new PluginOwnerId(ownerId));
        foreach (var plugin in plugins)
            owner.RegisterPlugin(plugin.name, plugin.description, plugin.functions);
        
        owner.ClearDomainEvents();
        return owner;
    }
  
}