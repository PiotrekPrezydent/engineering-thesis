using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Mediation;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Messaging;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Processing;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.References;
using Dara.Server.Modules.Orchestration.Application;
using Dara.Server.Modules.Orchestration.Application.AgentRuns.GenerateExecutionPlan;
using Dara.Server.Modules.Orchestration.Infrastructure.AgentRuns.Generation;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.Modules.Orchestration.Infrastructure;

public class OrchestrationCompositionRoot : ModuleCompositionRootBase
{
    private readonly IReadOnlyList<IChatClientProvider> _chatClientProviders;
    private readonly ExecutionPlanGeneratorOptions _generatorOptions;

    public OrchestrationCompositionRoot() : this([new GoogleChatClientProvider()], new ExecutionPlanGeneratorOptions())
    {
    }

    // tests give a scripted provider, so no test calls the api of a model provider
    public OrchestrationCompositionRoot(IReadOnlyList<IChatClientProvider> chatClientProviders, ExecutionPlanGeneratorOptions generatorOptions)
    {
        _chatClientProviders = chatClientProviders;
        _generatorOptions = generatorOptions;
    }

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

    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(_generatorOptions);
        foreach (var provider in _chatClientProviders)
            services.AddSingleton(provider);

        services.AddScoped<IExecutionPlanGenerator, ExecutionPlanGenerator>();
    }
}