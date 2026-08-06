using System.Reflection;
using Dara.Server.BuildingBlocks.Application;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Types;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Visitors;
using Dara.Shared.Attributes;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration.References;

[GenerateBuilder]
public partial class ModuleReferencesDescriptor : IVisitable<ModuleReferencesDescriptor>
{
    public Assembly ApplicationAssembly { get; set; }
    public Assembly InfrastructureAssembly { get; set;  }
    
    public IReadOnlyList<Type> MediationOpenTypes { get; set; }
    
    public IReadOnlyList<Type> TypeWiseDecorators { get; set; }
    
    [ObsoleteMethodOnRepeatedType(typeof(IModule))]
    public ITypeKey<IModule> DeclaredModuleInterface { get; set;  }

    public void Accept(IVisitor<ModuleReferencesDescriptor> visitor)
    {
        visitor.Visit(this);
    }
}