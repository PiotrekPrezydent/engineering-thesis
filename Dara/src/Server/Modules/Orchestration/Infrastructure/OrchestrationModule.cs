using Dara.Server.BuildingBlocks.Infrastructure;
using Dara.Server.BuildingBlocks.Infrastructure.Mediation.HandlerResolving;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Commands;
using Dara.Server.Modules.Orchestration.Application;

namespace Dara.Server.Modules.Orchestration.Infrastructure;

public class OrchestrationModule : ModuleBase, IOrchestrationModule
{
    public OrchestrationModule(ICommandExecutor commandExecutor, IHandlersResolver handlersResolver) : base(commandExecutor, handlersResolver)
    {
    }
}