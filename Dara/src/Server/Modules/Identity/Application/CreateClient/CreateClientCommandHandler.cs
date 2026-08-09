using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Domain;

namespace Dara.Server.Modules.Identity.Application.CreateClient;

public class CreateClientCommandHandler : ICommandHandler<CreateClientCommand, Guid>
{
    private IClientRepository _clientRepository;
    
    public CreateClientCommandHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }
    
    public async Task<Guid> HandleAsync(CreateClientCommand command)
    {
        var client = Client.Create(command.ClientIdentifier);
        await _clientRepository.AddAsync(client);
        
        return client.ClientId.Value;
    }
}