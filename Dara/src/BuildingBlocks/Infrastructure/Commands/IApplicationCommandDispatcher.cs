using Dara.BuildingBlocks.Application;
using Dara.BuildingBlocks.Application.Commands;

namespace Dara.BuildingBlocks.Infrastructure.Commands
{
    public interface IApplicationCommandDispatcher
    {
        //Task<CommandResult> DispatchAsync<TCommand>(TCommand command) where TCommand : IApplicationCommand;

        Task<CommandResult> DispatchAsync<TCommand, TCommandResult>(TCommand command)
            where TCommand : IModuleCommand
            where TCommandResult : IModuleCommandResult;
    }
}