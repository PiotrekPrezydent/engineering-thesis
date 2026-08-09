using Dara.Server.BuildingBlocks.Application.Queries;

namespace Dara.Server.Modules.Identity.Application.GetClient;

public class GetClientQueryHandler : IQueryHandler<GetClientQuery, ClientDto>
{
    private readonly IClientQueries _clientQueries;

    public GetClientQueryHandler(IClientQueries clientQueries)
    {
        _clientQueries = clientQueries;
    }
    
    public async Task<ClientDto> HandleAsync(GetClientQuery query)
    {
        var client = await  _clientQueries.GetClientByIdentifierAsync(query.ClientIdentifier);
        if (client == null)
            return new(Guid.Empty);

        return new(client.ClientId.Value);
    }
}