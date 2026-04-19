using Dara.BuildingBlocks.Application;

namespace Dara.Modules.Communication.Application.Clients;

public record CreateClientCommand() : IApplicationCommand;

public record CreateClientCommandResult() : IApplicationCommandResult;

public class CreateClientCommandHandler : IApplicationCommandHandler<CreateClientCommand, CreateClientCommandResult>
{
    public CreateClientCommandHandler()
    {
    }

    public async Task<CreateClientCommandResult> HandleAsync(CreateClientCommand command)
    {
        return new();
    }
}