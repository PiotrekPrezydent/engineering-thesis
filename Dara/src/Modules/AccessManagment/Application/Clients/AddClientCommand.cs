using Dara.BuildingBlocks.Domain.Commands;
using Dara.Modules.AccessManagment.Domain.Clients;

namespace Dara.Modules.AccessManagment.Application.Clients;

public record AddClientCommand(string name, string token) : IApplicationCommand;

public record AddClientCommandResult(Guid Id) : IApplicationCommandResult;

public class AddClientCommandHandler : IApplicationCommandHandler<AddClientCommand,AddClientCommandResult>
{
    private readonly IClientRepository _clientRepository;
    
    public AddClientCommandHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }
    
    public async Task<AddClientCommandResult> HandleAsync(AddClientCommand command)
    {
        ClientName name = new(command.name);
        ClientToken token = new(command.token);
        
        Client client = new(name, token);
        
        await _clientRepository.Add(client);
        
        return new AddClientCommandResult(client.Id);
    }
}