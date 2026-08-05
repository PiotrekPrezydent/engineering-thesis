using System.Reflection;
using Dara.Server.BuildingBlocks.Application;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Types;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Visitors;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.CompositionRoot;
using Dara.Shared.Attributes;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration.ModuleDescriptors;

[GenerateBuilder]
public partial class ModuleReferencesDescriptor : IVisitable<ModuleReferencesDescriptor>
{
    public Assembly ApplicationAssembly { get; set; }
    public Assembly InfrastructureAssembly { get; set;  }
    
    [ObsoleteMethodOnRepeatedType(typeof(IModule))]
    public ITypeKey<IModule> DeclaredModule { get; set;  }

    [ObsoleteMethodOnRepeatedType(typeof(IModuleCompositionRoot))]
    public ITypeKey<IModuleCompositionRoot> ModuleCompositionRoot { get; set;  }

    public void Accept(IVisitor<ModuleReferencesDescriptor> visitor)
    {
        visitor.Visit(this);
    }
}