using Dara.Server.BuildingBlocks.Infrastructure.Common.Types;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Visitors;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Shared.Attributes;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration.DataAccess;

[GenerateBuilder]
public partial class ModuleDataAccessConfiguration : IVisitable<ModuleDataAccessConfiguration>
{
    [ObsoleteMethodOnRepeatedType(typeof(DbContext))]
    public ITypeKey<DbContext> ModuleContext { get; set; }
    
    [ObsoleteMethodOnRepeatedType(typeof(IUnitOfWork))]
    public ITypeKey<IUnitOfWork> UnitOfWork { get; set; }
    
    public void Accept(IVisitor<ModuleDataAccessConfiguration> visitor)
    {
        visitor.Visit(this);
    }
}