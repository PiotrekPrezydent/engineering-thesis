using Dara.Server.BuildingBlocks.Infrastructure.Common.Types;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Visitors;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging;
using Dara.Shared.Attributes;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration.DataAccess;

[GenerateBuilder]
public partial class ModuleDataAccessDescriptor : IVisitable<ModuleDataAccessDescriptor>
{
    [ObsoleteMethodOnRepeatedType(typeof(ModuleContext))]
    public ITypeKey<ModuleContext> ModuleContext { get; set; }
    
    public void Accept(IVisitor<ModuleDataAccessDescriptor> visitor)
    {
        visitor.Visit(this);
    }
}