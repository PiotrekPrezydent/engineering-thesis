using Dara.Server.BuildingBlocks.Infrastructure;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.CompositionRoot;
using Dara.Server.Modules.Groups.Application;

namespace Dara.Server.Modules.Groups.Infrastructure;

public class GroupModule : ModuleBase, IGroupModule
{
    public GroupModule(IModuleCompositionRoot compositionRoot) : base(compositionRoot)
    {
    }
}