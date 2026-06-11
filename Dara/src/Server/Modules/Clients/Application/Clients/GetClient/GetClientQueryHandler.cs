using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.Modules.Clients.Domain.Clients;

namespace Dara.Server.Modules.Clients.Application.Clients.GetClient;

public class GetClientQueryHandler : IQueryHandler<GetClientQuery,ClientDto>
{
    IClientRepository _clientRepository;
    public GetClientQueryHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }
    
    public async Task<ClientDto> HandleAsync(GetClientQuery query)
    {
        var clientId = new ClientId(query.ClientId);
        var client = await _clientRepository.GetByClientByIdAsync(clientId);
        
        return new(client.Id.Value, client.Name, client.IsOnline);
    }
}