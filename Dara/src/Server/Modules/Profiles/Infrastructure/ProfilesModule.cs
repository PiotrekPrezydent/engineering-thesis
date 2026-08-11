using Dara.Server.BuildingBlocks.Infrastructure;
using Dara.Server.BuildingBlocks.Infrastructure.Mediation.HandlerResolving;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Commands;
using Dara.Server.Modules.Profiles.Application;

namespace Dara.Server.Modules.Profiles.Infrastructure;

public class ProfilesModule : ModuleBase,  IProfilesModule
{
    public ProfilesModule(ICommandExecutor commandExecutor, IHandlersResolver handlersResolver) : base(commandExecutor, handlersResolver)
    {
    }
}