using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Mediation;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Messaging;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Processing;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.References;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.SignIn;
using Dara.Server.Modules.Identity.Application.AuthenticatePanelSignIn;
using Dara.Server.Modules.Identity.Domain.Users;
using Dara.Server.Modules.Identity.Infrastructure.UserSessions;
using Dara.Server.Modules.Identity.Infrastructure.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.Modules.Identity.Infrastructure;

public class IdentityCompositionRoot : ModuleCompositionRootBase
{
    private readonly IdentityOptions _options;

    public IdentityCompositionRoot() : this(new IdentityOptions())
    {
    }

    // tests give short lifetimes (refresh token, codes, panel sign-in) to check the expiry
    public IdentityCompositionRoot(IdentityOptions options)
    {
        _options = options;
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

    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(_options);
        services.AddSingleton<IPasswordHasher, AspNetPasswordHasher>();
        services.AddSingleton<IRefreshTokenFactory, RefreshTokenFactory>();
        services.AddSingleton<IClientCodeFactory, ClientCodeFactory>();
        services.AddScoped<IUserUniquenessChecker, UserUniquenessChecker>();
        services.AddScoped<CredentialsVerifier>();
        services.AddScoped<PanelSignInAuthenticator>();
    }
}
