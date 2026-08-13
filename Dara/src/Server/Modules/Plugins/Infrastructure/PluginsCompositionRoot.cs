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

    // protected override void OnServiceProviderCreated(IServiceProvider serviceProvider)
    // {
    //     var context = serviceProvider.GetRequiredService<PluginsContext>();
    //     context.Database.EnsureCreated();
    //
    //     context.PluginsConfigurations.AddRange(
    //         SeedPluginOwner(SharedSeedGuids.User1, 
    //             ("AudioControl", "", PluginsSeed.GetAudioControlFunctions()),
    //             ("PowerManagement","",PluginsSeed.GetPowerManagementFunctions()),
    //             ("CrossDeviceHandoff","",PluginsSeed.GetCrossDeviceHandoffFunctions()),
    //             ("VoiceInteraction","",PluginsSeed.GetVoiceInteractionFunctions())
    //         ),
    //         SeedPluginOwner(SharedSeedGuids.User2, 
    //             ("Telephony", "", PluginsSeed.GetTelephonyFunctions()),
    //             ("DeviceLocator", "", PluginsSeed.GetDeviceLocatorFunctions()),
    //             ("RemoteSensor","",PluginsSeed.GetRemoteSensorFunctions()),
    //             ("RemoteAuthentication","",PluginsSeed.GetRemoteAuthenticationFunctions()),
    //             ("NotificationSync","",PluginsSeed.GetNotificationSyncFunctions()),
    //             ("CrossDeviceHandoff","",PluginsSeed.GetCrossDeviceHandoffFunctions())
    //         ),
    //         SeedPluginOwner(SharedSeedGuids.User3, 
    //             ("AudioControl", "", PluginsSeed.GetAudioControlFunctions()),
    //             ("VoiceInteraction","",PluginsSeed.GetVoiceInteractionFunctions()),
    //             ("RemoteSensor","",PluginsSeed.GetRemoteSensorFunctions())
    //         ),
    //         SeedPluginOwner(SharedSeedGuids.User4, 
    //             ("PowerManagement", "", PluginsSeed.GetPowerManagementFunctions())
    //         ),
    //         SeedPluginOwner(SharedSeedGuids.User5, 
    //             ("AudioControl", "",PluginsSeed.GetAudioControlFunctions()), 
    //             ("CrossDeviceHandoff", "", PluginsSeed.GetCrossDeviceHandoffFunctions())
    //         )
    //     );
    //     
    //     context.SaveChanges();
    // }
    
    PluginOwner SeedPluginOwner(Guid ownerId, params (string name, string description, List<PluginFunction> functions)[] plugins)
    {
        var owner = PluginOwner.CreateDefault(new PluginOwnerId(ownerId));
        foreach (var plugin in plugins)
            owner.AddPlugin(plugin.name, plugin.description, plugin.functions);
        
        owner.ClearDomainEvents();
        return owner;
    }
  
}