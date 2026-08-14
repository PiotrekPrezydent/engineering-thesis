using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.Common;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Types;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Logging;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Mediation;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Messaging;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.Processing;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.References;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Mediation.Decorators;
using Dara.Server.BuildingBlocks.Infrastructure.Mediation.HandlerResolving;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Mapping;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Persistence;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Processing;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Persistence;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Processing;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Commands;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.DomainEvents;
using Dara.Server.BuildingBlocks.Integration;
using Dara.Shared.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
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

    public AsyncServiceScope CreateAsyncScope()
    {
        if(_services == null)
            throw  new InvalidOperationException($"{GetType().FullName} is not initialized.");
        
        return _services.CreateAsyncScope();
    }

    private void SetServiceProvider(IServiceProvider serviceProvider)
    {
        _services = serviceProvider;
    }

    public void Initialize(IServiceCollection rootServices, IEventBus eventBus)
    {
        IServiceCollection services = new ServiceCollection();

        ModuleReferencesConfiguration.ModuleReferencesConfigurationBuilder referencesBuilder = new();
        ModuleDataAccessConfiguration.ModuleDataAccessConfigurationBuilder dataAccessBuilder = new();
        ModuleMediationConfiguration.ModuleMediationConfigurationBuilder mediationBuilder = new();
        ModuleProcessingConfiguration.ModuleProcessingConfigurationBuilder processingBuilder = new();
        ModuleMessagingConfiguration.ModuleMessagingConfigurationBuilder messagingBuilder = new();
        
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
        
        var moduleDeclaration = references.InfrastructureAssembly.GetFirstImplementationOfType(references.DeclaredModuleInterface.Value);
        services.AddScoped(moduleDeclaration.Interface,moduleDeclaration.Implementation);

        DateTime startTime = DateTime.UtcNow;
        services.AddLogging(e =>
        {
            e.ClearProviders();
            e.AddProvider(new ModuleLoggerProvider(this.GetModuleName()));
            e.AddFilter((category, level) =>
            {
                if (DateTime.UtcNow - startTime < TimeSpan.FromSeconds(2))
                {
                    if (category != null && category.StartsWith("Microsoft.EntityFrameworkCore"))
                    {
                        return false;
                    }
                }

                if (level < LogLevel.Information)
                    return false;

                return true;
            });
        });
        
        
        var declaredHandlers = references.ApplicationAssembly.GetImplementationsOfOpenGeneric(typeof(IIntegrationEventHandler<>));
        var inboxMap = new BiDictionary<Type, string>();
        foreach (var handler in declaredHandlers)
        {
            var eventType = handler.Interface.GenericTypeArguments[0];
            inboxMap.Add(eventType, eventType.Name);
            
            var eventTypeKey = Activator.CreateInstance(typeof(TypeKey<>).MakeGenericType(eventType)) as ITypeKey<IIntegrationEvent>;
            
            eventTypeKey!.ExecuteGenericAction(new IntegrationEventRegistrator(references.CompositionRoot,eventBus));
        }
        
        services.AddSingleton<IInboxMessagesTypeMapper>(new InboxMessagesTypeMapper(inboxMap));
        
        services.AddSingleton(eventBus);
        services.AddSingleton(references.CompositionRoot);

        services.AddLogging(e =>
        {
            e.ClearProviders();
            e.AddProvider(new ModuleLoggerProvider(this.GetModuleName()));
        });

        // services.AddLogging(e =>
        // {
        //     e.ClearProviders();
        //     e.AddConsole(options =>
        //     {
        //         options.FormatterName = nameof(SharedLogFormatter);
        //     }).AddConsoleFormatter<SharedLogFormatter, ConsoleFormatterOptions>();
        // });
        
        SetServiceProvider(services.BuildServiceProvider());

        using (var scope = CreateScope())
        {
            OnServiceProviderCreated(scope.ServiceProvider);
        }
        
        rootServices.AddScoped(moduleDeclaration.Interface , _ => _services.GetRequiredService(moduleDeclaration.Interface));
        
        rootServices.AddSingleton<IHostedService>(_ => _services.GetRequiredService<OutboxBackgroundWorker>());
        rootServices.AddSingleton<IHostedService>(_ => _services.GetRequiredService<InboxBackgroundWorker>());
    }

    class IntegrationEventRegistrator(IModuleCompositionRoot compositionRoot, IEventBus eventBus) : IKeyedTypeAction<IIntegrationEvent>
    {
        public void Execute<TType>(ITypeKey<IIntegrationEvent> typeKey) where TType : IIntegrationEvent
        {
            var handler = new InboxWriterIntegrationEventHandler<TType>(compositionRoot);
            eventBus.Subscribe(handler);
        }
    }

    protected virtual void OnServiceProviderCreated(IServiceProvider serviceProvider)
    {
    }

    protected abstract void ConfigureReferences(ModuleReferencesConfiguration.ModuleReferencesConfigurationBuilder builder);
    
    protected abstract void ConfigureDataAccess(ModuleDataAccessConfiguration.ModuleDataAccessConfigurationBuilder builder);
    
    protected abstract void ConfigureMediation(ModuleMediationConfiguration.ModuleMediationConfigurationBuilder builder);
    
    protected abstract void ConfigureProcessing(ModuleProcessingConfiguration.ModuleProcessingConfigurationBuilder builder);
    
    protected abstract void ConfigureMessaging(ModuleMessagingConfiguration.ModuleMessagingConfigurationBuilder builder);
    
    protected void AddStandardDataAccess<TContext>(ModuleDataAccessConfiguration.ModuleDataAccessConfigurationBuilder builder) where TContext : DbContext, IReadModel
    {
        builder
            .WithUnitOfWork(ITypeKey<UnitOfWork>.Instance)
            .WithReadModel(ITypeKey<TContext>.Instance)
            .WithModuleContext(ITypeKey<TContext>.Instance);
    }
    
    protected void AddStandardMediation(ModuleMediationConfiguration.ModuleMediationConfigurationBuilder builder)
    {
        builder
            .ConfigureMediationOpenTypes(e => e
                .Add(typeof(ICommandHandler<>))
                .Add(typeof(ICommandHandler<,>))
                .Add(typeof(IQueryHandler<,>))
                .Add(typeof(IDomainEventHandler<>))
                .Add(typeof(IDomainEventNotificationHandler<>))
                .Add(typeof(IIntegrationEventHandler<>)))
            .ConfigureTypeWiseDecorators(e => e
                .Add(typeof(CommandHandlerUnitOfWorkDecorator<,>))
                .Add(typeof(CommandHandlerUnitOfWorkDecorator<>)))
            .WithHandlersResolver(ITypeKey<HandlersResolver>.Instance);
    }

    protected void AddStandardProcessing(ModuleProcessingConfiguration.ModuleProcessingConfigurationBuilder builder)
    {
        builder
            .WithCommandExecutor(ITypeKey<CommandExecutor>.Instance)
            .WithDomainEventDispatcher(ITypeKey<DomainEventsDispatcher>.Instance);
    }

    protected void AddStandardMessaging<TContext>(ModuleMessagingConfiguration.ModuleMessagingConfigurationBuilder builder) where TContext : DbContext
    {
        builder
            .WithDomainNotificationOpenGenericType(typeof(IDomainEventNotification<>))
            .WithInboxRepository(ITypeKey<InboxRepository<TContext>>.Instance)
            .WithInboxProcessor(ITypeKey<InboxMessageProcessor>.Instance)
            .WithOutboxRepository(ITypeKey<OutboxRepository<TContext>>.Instance)
            .WithOutboxProcessor(ITypeKey<OutboxMessageProcessor>.Instance);
    }
}



