using Dara.BuildingBlocks.Application;

namespace Dara.Modules.Communication.Application.Clients;

public record DeleteClientCommand() : IApplicationCommand;

public record DeleteClientCommandResult() : IApplicationCommandResult;

public class DeleteClientCommandHandler : IApplicationCommandHandler<DeleteClientCommand, DeleteClientCommandResult>
{
    public DeleteClientCommandHandler()
    {
    }

    public async Task<DeleteClientCommandResult> HandleAsync(DeleteClientCommand command)
    {
        return new();
    }
}