using Dara.Server.BuildingBlocks.Infrastructure.Common.Visitors;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.CompositionRoot;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.ModuleDescriptors;
using Dara.Server.BuildingBlocks.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration.Visitors;

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

        var imple = instance.InfrastructureAssembly.GetFirstImplementationOfType(instance.DeclaredModuleInterface.Value);
        
        _serviceCollection.AddScoped(imple.Interface, imple.Implementation);
    }
}