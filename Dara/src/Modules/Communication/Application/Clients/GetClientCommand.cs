using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.Clients;

public record GetClientCommand() : IApplicationCommand;

public record GetClientCommandResult() : IApplicationCommandResult;

public class GetClientCommandHandler : IApplicationCommandHandler<GetClientCommand, GetClientCommandResult>
{
    public GetClientCommandHandler()
    {
    }

    public async Task<GetClientCommandResult> HandleAsync(GetClientCommand command)
    {
        return new();
    }
}