using Dara.Server.BuildingBlocks.Infrastructure;
using Dara.Server.BuildingBlocks.Infrastructure.Mediation.HandlerResolving;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Commands;
using Dara.Server.Modules.Groups.Application;

namespace Dara.Server.Modules.Groups.Infrastructure;

public class GroupsModule : ModuleBase, IGroupsModule
{
    public GroupsModule(ICommandExecutor commandExecutor, IHandlersResolver handlersResolver) : base(commandExecutor, handlersResolver)
    {
    }
}