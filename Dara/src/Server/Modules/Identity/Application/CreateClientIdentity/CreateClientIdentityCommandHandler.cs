using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Domain;

namespace Dara.Server.Modules.Identity.Application.CreateClientIdentity;

public class CreateClientIdentityCommandHandler : ICommandHandler<CreateClientIdentityCommand, Guid>
{
    private IClientIdentityRepository _clientIdentityRepository;
    
    public CreateClientIdentityCommandHandler(IClientIdentityRepository clientIdentityRepository)
    {
        _clientIdentityRepository = clientIdentityRepository;
    }
    
    public async Task<Guid> HandleAsync(CreateClientIdentityCommand identityCommand)
    {
        Console.WriteLine("HHANDLE CREATE");
        var client = ClientIdentity.Create(identityCommand.ClientIdentifier);
        await _clientIdentityRepository.AddAsync(client);
        
        return client.ClientId.Value;
    }
}