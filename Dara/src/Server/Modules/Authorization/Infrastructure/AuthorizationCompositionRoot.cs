using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Mediation;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Messaging;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Processing;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.References;
using Dara.Server.Modules.Authorization.Application;
using Dara.Server.Modules.Authorization.Domain.Permissions;
using Dara.Server.Modules.Authorization.Domain.Roles;
using Dara.Server.Modules.Authorization.Domain.Users;
using Dara.Server.Modules.Authorization.Infrastructure.Permissions;
using Dara.Server.Modules.Authorization.Infrastructure.Roles;
using Dara.Server.Modules.Authorization.Infrastructure.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.Modules.Authorization.Infrastructure;

public class AuthorizationCompositionRoot : ModuleCompositionRootBase
{
    protected override void ConfigureReferences(ModuleReferencesConfiguration.ModuleReferencesConfigurationBuilder builder)
    {
        builder
            .WithApplicationAssembly(IAuthorizationModule.ContainingAssembly)
            .WithInfrastructureAssembly(AuthorizationModule.ContainingAssembly)
            .WithDeclaredModuleInterface(IAuthorizationModule.AsTypeKey)
            .WithCompositionRoot(this);
    }

    protected override void ConfigureDataAccess(ModuleDataAccessConfiguration.ModuleDataAccessConfigurationBuilder builder)
    {
        AddStandardDataAccess<AuthorizationContext>(builder);
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
        AddStandardMessaging<AuthorizationContext>(builder);
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRoleExistenceChecker, RoleExistenceChecker>();
        services.AddScoped<IUserRolesRepository, UserRolesRepository>();
    }
}
