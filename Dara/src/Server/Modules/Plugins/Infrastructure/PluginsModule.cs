using Dara.Server.BuildingBlocks.Infrastructure;
using Dara.Server.BuildingBlocks.Infrastructure.Mediation.HandlerResolving;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Commands;
using Dara.Server.Modules.Plugins.Application;

namespace Dara.Server.Modules.Plugins.Infrastructure;

public class PluginsModule : ModuleBase, IPluginsModule
{
    public PluginsModule(ICommandExecutor commandExecutor, IHandlersResolver handlersResolver) : base(commandExecutor, handlersResolver)
    {
    }
}