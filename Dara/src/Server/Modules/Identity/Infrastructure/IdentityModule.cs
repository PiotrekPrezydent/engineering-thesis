using Dara.Server.BuildingBlocks.Infrastructure;
using Dara.Server.BuildingBlocks.Infrastructure.Mediation.HandlerResolving;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Commands;
using Dara.Server.Modules.Identity.Application;

namespace Dara.Server.Modules.Identity.Infrastructure;

public class IdentityModule : ModuleBase, IIdentityModule
{
    public IdentityModule(ICommandExecutor commandExecutor, IHandlersResolver handlersResolver) : base(commandExecutor, handlersResolver)
    {
    }
}