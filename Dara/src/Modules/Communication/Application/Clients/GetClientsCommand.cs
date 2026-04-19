using Dara.BuildingBlocks.Application;

namespace Dara.Modules.Communication.Application.Clients;

public record GetClientsCommand() : IApplicationCommand;

public record GetClientsCommandResult() : IApplicationCommandResult;

public class GetClientsCommandHandler : IApplicationCommandHandler<GetClientsCommand, GetClientsCommandResult>
{
    public GetClientsCommandHandler()
    {
    }

    public async Task<GetClientsCommandResult> HandleAsync(GetClientsCommand command)
    {
        return new();
    }
}