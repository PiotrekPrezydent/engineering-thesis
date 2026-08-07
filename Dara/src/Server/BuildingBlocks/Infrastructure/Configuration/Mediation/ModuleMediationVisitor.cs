using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Visitors;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.References;
using Dara.Server.BuildingBlocks.Infrastructure.Mediation.HandlerResolving;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Mapping;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration.Mediation;

public class ModuleMediationVisitor : IVisitor<ModuleMediationConfiguration>
{
    private readonly ModuleReferencesConfiguration _referencesConfiguration;
    private readonly IServiceCollection _serviceCollection;

    public ModuleMediationVisitor(ModuleReferencesConfiguration referencesConfiguration, IServiceCollection serviceCollection)
    {
        _referencesConfiguration = referencesConfiguration;
        _serviceCollection = serviceCollection;
    }

    public void Visit(ModuleMediationConfiguration instance)
    {
        _serviceCollection.AddScoped(typeof(IHandlersResolver), instance.HandlersResolver.Value);
        
        var mediationOpenTypes = instance.MediationOpenTypes;
        
        foreach (var mediationType in mediationOpenTypes)
        {
            var implementations = _referencesConfiguration.ApplicationAssembly.GetImplementationsOfOpenGeneric(mediationType);
            foreach (var implementation in implementations)
            {
                _serviceCollection.AddTransient(implementation.Interface, implementation.Implementation);
            }
        }
        
        foreach (var decorator in instance.TypeWiseDecorators)
        {
            _serviceCollection.AddTypeWiseDecorator(decorator);
        }
    }
}