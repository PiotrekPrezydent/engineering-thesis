using Dara.BuildingBlocks.Domain.Commands;
using Dara.Modules.AccessManagment.Domain.Clients;

namespace Dara.Modules.AccessManagment.Application.Clients;

public record GetClientCommand(string Name) : IApplicationCommand;

public record GetClientCommandResult(Guid Id) : IApplicationCommandResult;

public class GetClientCommandHandler  : IApplicationCommandHandler<GetClientCommand,GetClientCommandResult>
{    
    private readonly IClientRepository _clientRepository;

    public GetClientCommandHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<GetClientCommandResult> HandleAsync(GetClientCommand command)
    {
        ClientName name = new(command.Name);
        Client client = await _clientRepository.FindByName(name);

        return new GetClientCommandResult(client.Id);
    }
}