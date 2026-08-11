using Dara.Server.BuildingBlocks.Infrastructure.Common.Types;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Visitors;
using Dara.Server.BuildingBlocks.Infrastructure.Mediation.HandlerResolving;
using Dara.Shared.Attributes;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration.Mediation;

[GenerateBuilder]
public partial class ModuleMediationConfiguration : IVisitable<ModuleMediationConfiguration>
{
    public IReadOnlyList<Type> MediationOpenTypes { get; set; }
    
    public IReadOnlyList<Type> TypeWiseDecorators { get; set; }
    
    [ObsoleteMethodOnRepeatedType(typeof(IHandlersResolver))]
    public ITypeKey<IHandlersResolver> HandlersResolver { get; set; }

    public void Accept(IVisitor<ModuleMediationConfiguration> visitor)
    {
        visitor.Visit(this);
    }
}