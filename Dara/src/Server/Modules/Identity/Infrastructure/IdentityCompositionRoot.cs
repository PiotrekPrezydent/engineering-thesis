using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Domain.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Mediation;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Messaging;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Processing;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.References;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess.DomainEventsMapping;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Domain;
using Dara.Server.Modules.Identity.Domain.Events;
using Microsoft.Extensions.DependencyInjection;

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

    public async Task PublishSeedUsersCreated()
    {
        using var scope = CreateScope();
        var bus = scope.ServiceProvider.GetRequiredService<IEventBus>();
        var context = scope.ServiceProvider.GetRequiredService<IdentityContext>();
        var domainMapper = scope.ServiceProvider.GetRequiredService<IDomainEventNotificationMapper>();
      
        
        var users = context.Users.ToList();
        // foreach (var user in users)
        // {
        //     var createdEvent = new NewUserCreatedDomainEvent(user.Id);
        //     var notificationType = domainMapper.GetNotificationTypeForDomainEvent(createdEvent.GetType());
        //     var notification = Activator.CreateInstance(notificationType, createdEvent.EventId, createdEvent) as IDomainEventNotification<IDomainEvent>;
        //     
        //     
        //     
        //     await bus.PublishAsync(new )
        // }
    }
}