using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Visitors;
using Dara.Server.BuildingBlocks.Infrastructure.Extensions;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Commands;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration.References;

public class ModuleReferencesVisitor : IVisitor<ModuleReferencesDescriptor>
{
    private readonly IServiceCollection _serviceCollection;

    public ModuleReferencesVisitor(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public void Visit(ModuleReferencesDescriptor instance)
    {
        var mediationOpenTypes = instance.MediationOpenTypes;
        
        foreach (var mediationType in mediationOpenTypes)
        {
            var implementations = instance.ApplicationAssembly.GetImplementationsOfOpenGeneric(mediationType);
            foreach (var implementation in implementations)
            {
                _serviceCollection.AddTransient(implementation.Interface, implementation.Implementation);
            }
        }
        
        Dictionary<string, Type> outboxMap = new Dictionary<string, Type>();
        
        foreach (var consumer in instance.ApplicationAssembly.GetImplementationsOfOpenGeneric(typeof(IDomainEventNotificationHandler<>)))
        {
            _serviceCollection.AddTransient(consumer.Interface, consumer.Implementation);
            
            var eventType = consumer.Interface.GenericTypeArguments[0];
            
            if(outboxMap.ContainsValue(eventType))
                continue;
            
            outboxMap.Add(eventType.Name, eventType);
        }
        _serviceCollection.AddSingleton<IOutboxTypeMapper>(new OutboxTypeMapper(outboxMap));
        
        var repositories = instance.InfrastructureAssembly.GetTypes().Where(e=>typeof(IRepository).IsAssignableFrom(e)).ToList();
        foreach (var repository in repositories)
        {
            var implementedInterface = repository.GetInterfaces().First();
            _serviceCollection.AddScoped(implementedInterface, repository);
        }
        
        foreach (var decorator in instance.TypeWiseDecorators)
        {
            _serviceCollection.AddTypeWiseDecorator(decorator);
        }

        var imple = instance.InfrastructureAssembly.GetFirstImplementationOfType(instance.DeclaredModuleInterface.Value);
        
        _serviceCollection.AddScoped(imple.Interface, imple.Implementation);
    }
}