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
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Mapping;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Persistence;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Processing;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox;
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

    public void LogBenchmark()
    {
        using var scope = CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DbContext>();

        var outboxMessages = context.Set<OutboxMessage>().AsNoTracking();
        var inboxMessages = context.Set<InboxMessage>().AsNoTracking();

        TimeSpan outboxData = TimeSpan.Zero;
        TimeSpan inboxData = TimeSpan.Zero;

        foreach (var message in outboxMessages)
        {
            if(message.ProcessedDate == null)
                continue;

            outboxData += (message.ProcessedDate! - message.OccurredOn).Value;
        }

        foreach (var message in inboxMessages)
        {
            if(message.ProcessedDate == null)
                continue;

            inboxData += (message.ProcessedDate! - message.OccurredOn).Value;
        }
        
        var logger = _services.GetRequiredService<ILogger<ModuleCompositionRootBase>>();

        var outboxAvg = outboxMessages.Any() ? outboxData.Ticks / outboxMessages.Count() : int.MinValue;
        var outboxSecs = outboxMessages.Any() ? outboxData.TotalSeconds / outboxMessages.Count() : int.MinValue;
        
        var inboxAvg = inboxMessages.Any() ? inboxData.Ticks / inboxMessages.Count() : int.MinValue;
        var inboxSecs =inboxMessages.Any() ? inboxData.TotalSeconds / inboxMessages.Count() : int.MinValue;
        
        logger.LogInformation("OUTBOX AVERAGE TICKS : " + outboxAvg.ToString() + " --- " + outboxSecs + " --- " + outboxMessages.Count());
        logger.LogInformation("INBOX AVERAGE TICKS : " + inboxAvg.ToString() + " --- " + inboxSecs + " --- " + inboxMessages.Count());
        
        /*
         [01:40:27] [INF] [OUTBOX MESSAGE PROCESSOR :::: Identity] [Identity]
           [.8171] STARTING PROCESSING OUTBOX MESSAGE NewUserCreatedNotification
           [.8554] PROCESSED OUTBOX MESSAGE NewUserCreatedNotification IN 0.4123323
           
           the first outbox message to happend is slow af, propably bcs i use in memory db base
           
         *[01:40:35] [INF] [Dara.Server.BuildingBlocks.Infrastructure.Configuration.ModuleCompositionRootBase] [Plugins]
           [.9725] OUTBOX AVERAGE TICKS : 217412 --- 0.021741255
           [.9725] INBOX AVERAGE TICKS : 132876 --- 0.01328768
           
         *01:40:35] [INF] [Dara.Server.BuildingBlocks.Infrastructure.Configuration.ModuleCompositionRootBase] [Orchestration]
           [.9787] OUTBOX AVERAGE TICKS : -2147483648 --- -2147483648
           [.9787] INBOX AVERAGE TICKS : 215854 --- 0.021585483333333332
           
           plugins is publishing fat event which orchestration is consuming, idk what to do about that, 
           maybe dont remove functions in orchestration and cache them with mark to delete later to prevent lagging on adding / removing plugins 
           
         * [01:40:35] [INF] [Dara.Server.BuildingBlocks.Infrastructure.Configuration.ModuleCompositionRootBase] [Identity]
           [.9495] OUTBOX AVERAGE TICKS : 1521910 --- 0.15219102
           [.9496] INBOX AVERAGE TICKS : -2147483648 --- -2147483648
           
           [01:40:35] [INF] [Dara.Server.BuildingBlocks.Infrastructure.Configuration.ModuleCompositionRootBase] [Profiles]
           [.9650] OUTBOX AVERAGE TICKS : 119395 --- 0.01193952
           [.9650] INBOX AVERAGE TICKS : 113990 --- 0.011399040000000001
           
           [01:40:35] [INF] [Dara.Server.BuildingBlocks.Infrastructure.Configuration.ModuleCompositionRootBase] [Groups]
           [.9572] OUTBOX AVERAGE TICKS : 119177 --- 0.011917758333333334
           [.9572] INBOX AVERAGE TICKS : 104008 --- 0.0104008
                                                                                ENTRIES IN DB
            identity: [.1141] OUTBOX LOOP : 5 / Processor call: 4               5
                    [.1119]   INBOX LOOP : 0                                        0
            groups     [.0556] INBOX LOOP : 10 / Processor call: 9              10
                    [.1063] OUTBOX LOOP : 22 / [.1037] Processor call: 11       12
                    
            profiles [.1144] INBOX LOOP : 5 / [.1140] Processor call: 4         5
                    .0476] OUTBOX LOOP : 10 / [.0463] Processor call: 4         5
                    
            plugins: [.1141] INBOX LOOP : 5 / [.1138] Processor call: 4             5
                    [.1865] OUTBOX LOOP : 25 / [.1818] Processor call: 19           20
                    
            orchestration: [.1990] OUTBOX LOOP : 42 / no procesor calls             0
                        [.2026] INBOX LOOP : 42 / [.1983] Processor call: 41        42
                        
            the reason behind so many unnecessary outbox calls is because Unit of work is alway sending signal to refresh background worker, even if there is no notification to be written,
            idk if its making big performance problem, but it can be easly fix
         */
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



