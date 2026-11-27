using Dara.Server.BuildingBlocks.Infrastructure;
using Dara.Server.BuildingBlocks.Infrastructure.Mediation.HandlerResolving;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Commands;
using Dara.Server.Modules.Authorization.Application;

namespace Dara.Server.Modules.Authorization.Infrastructure;

public class AuthorizationModule : ModuleBase, IAuthorizationModule
{
    public AuthorizationModule(ICommandExecutor commandExecutor, IHandlersResolver handlersResolver) : base(commandExecutor, handlersResolver)
    {
    }
}
