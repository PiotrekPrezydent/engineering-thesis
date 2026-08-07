using System.Reflection;
using Dara.Server.BuildingBlocks.Application;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Types;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Visitors;
using Dara.Shared.Attributes;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration.References;

[GenerateBuilder]
public partial class ModuleReferencesConfiguration : IVisitable<ModuleReferencesConfiguration>
{
    public Assembly ApplicationAssembly { get; set; }
    public Assembly InfrastructureAssembly { get; set;  }
    
    public IModuleCompositionRoot CompositionRoot { get; set; }
    
    [ObsoleteMethodOnRepeatedType(typeof(IModule))]
    public ITypeKey<IModule> DeclaredModuleInterface { get; set;  }
    
    public void Accept(IVisitor<ModuleReferencesConfiguration> visitor)
    {
        visitor.Visit(this);
    }
}