using System.Text.Json;
using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.BuildingBlocks.Infrastructure.Common;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Types;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Mediation;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Messaging;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Processing;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.References;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Mediation.HandlerResolving;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Processing;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Processing;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Commands;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.DomainEvents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModuleMediationConfiguration = Dara.Server.BuildingBlocks.Infrastructure.Configuration.Mediation.ModuleMediationConfiguration;
using ModuleMessagingConfiguration = Dara.Server.BuildingBlocks.Infrastructure.Configuration.Messaging.ModuleMessagingConfiguration;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration;

public abstract class ModuleCompositionRootBase : IModuleCompositionRoot
{
    private IServiceProvider _services = null!;
    
    public IServiceScope CreateScope()
    { 
        if(_services == null)
            throw  new InvalidOperationException($"{GetType().FullName} is not initialized.");
        
        return _services.CreateScope();
    }

    private void SetServiceProvider(IServiceProvider serviceProvider)
    {
        _services = serviceProvider;
    }

    private record SpecialDomainEvent() : IDomainEvent;

    public void Initialize(IServiceCollection rootServices)
    {
        IServiceCollection services = new ServiceCollection();

        ModuleReferencesConfiguration.ModuleReferencesConfigurationBuilder referencesBuilder = new();
        ModuleDataAccessConfiguration.ModuleDataAccessConfigurationBuilder dataAccessBuilder = new();
        ModuleMediationConfiguration.ModuleMediationConfigurationBuilder mediationBuilder = new();
        ModuleProcessingConfiguration.ModuleProcessingConfigurationBuilder processingBuilder = new();
        ModuleMessagingConfiguration.ModuleMessagingConfigurationBuilder messagingBuilder = new();
        
        services.AddLogging(ConfigureLogging);
        ConfigureReferences(referencesBuilder);
        ConfigureDataAccess(dataAccessBuilder);
        ConfigureMediation(mediationBuilder);
        ConfigureProcessing(processingBuilder);
        ConfigureMessaging(messagingBuilder);
        
        var references = referencesBuilder.Build();
        var dataAccess = dataAccessBuilder.Build();
        var mediation = mediationBuilder.Build();
        var processing = processingBuilder.Build();
        var messaging = messagingBuilder.Build();
        
        ModuleMediationVisitor mediationVisitor = new(references,services);
        ModuleDataAccessVisitor dataAccessVisitor = new(references,services);
        ModuleProcessingVisitor processingVisitor = new(services);
        ModuleMessagingVisitor messagingVisitor = new(references,services);
        
        dataAccess.Accept(dataAccessVisitor);
        mediation.Accept(mediationVisitor);
        processing.Accept(processingVisitor);
        messaging.Accept(messagingVisitor);
        
        
        services.AddSingleton<IModuleCompositionRoot>(this);

        var moduleDeclaration =
            references.InfrastructureAssembly.GetFirstImplementationOfType(references.DeclaredModuleInterface.Value);
        
        services.AddScoped(moduleDeclaration.Interface,moduleDeclaration.Implementation);
        
        SetServiceProvider(services.BuildServiceProvider());

        
        rootServices.AddScoped(moduleDeclaration.Interface , _ => _services.GetRequiredService(moduleDeclaration.Interface));
        
        rootServices.AddSingleton<IHostedService>(_ => _services.GetRequiredService<OutboxBackgroundWorker>());
        rootServices.AddSingleton<IHostedService>(_ => _services.GetRequiredService<InboxBackgroundWorker>());
    }

    protected abstract void ConfigureLogging(ILoggingBuilder loggingBuilder);

    protected abstract void ConfigureReferences(ModuleReferencesConfiguration.ModuleReferencesConfigurationBuilder builder);
    
    protected abstract void ConfigureDataAccess(ModuleDataAccessConfiguration.ModuleDataAccessConfigurationBuilder builder);
    
    protected abstract void ConfigureMediation(ModuleMediationConfiguration.ModuleMediationConfigurationBuilder builder);
    
    protected abstract void ConfigureProcessing(ModuleProcessingConfiguration.ModuleProcessingConfigurationBuilder builder);
    
    protected abstract void ConfigureMessaging(ModuleMessagingConfiguration.ModuleMessagingConfigurationBuilder builder);

    protected static IReadOnlyList<Type> StandardMediationOpenTypes => new List<Type>
    {
        typeof(ICommandHandler<>),
        typeof(ICommandHandler<,>),
        typeof(IQueryHandler<,>),
        typeof(IDomainEventHandler<>),
    };
    
    protected static ITypeKey<DomainEventsDispatcher> StandardDomainEventsDispatcher => new TypeKey<DomainEventsDispatcher>();
    protected static ITypeKey<CommandExecutor> StandardCommandExecutor => new TypeKey<CommandExecutor>();
    protected static ITypeKey<HandlersResolver> StandardHandlersResolver => new TypeKey<HandlersResolver>();
    protected static ITypeKey<UnitOfWork> StandardUnitOfWork => new TypeKey<UnitOfWork>();
    
    protected static ITypeKey<OutboxProcessor> StandardOutboxProcessor => new TypeKey<OutboxProcessor>();
    
    protected static ITypeKey<InboxProcessor> StandardInboxProcessor => new TypeKey<InboxProcessor>();
}



