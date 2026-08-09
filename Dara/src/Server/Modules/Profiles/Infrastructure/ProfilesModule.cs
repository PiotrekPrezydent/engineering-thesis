using Dara.Server.BuildingBlocks.Infrastructure;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.Modules.Profiles.Application;

namespace Dara.Server.Modules.Profiles.Infrastructure;

public class ProfilesModule : ModuleBase,  IProfilesModule
{
    public ProfilesModule(IModuleCompositionRoot compositionRoot) : base(compositionRoot)
    {
    }
}