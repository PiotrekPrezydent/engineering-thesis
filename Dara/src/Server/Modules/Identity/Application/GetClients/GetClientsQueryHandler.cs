using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Identity.Application.Clients;
using Dara.Server.Modules.Identity.Domain.Clients;
using Dara.Server.Modules.Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Identity.Application.GetClients;

// clients of the hub only; the panel client of an account is a subject of Authorization, not a client to show
public class GetClientsQueryHandler :
    IQueryHandler<GetAccountClientsQuery, List<UserClientDto>>,
    IQueryHandler<GetAccountClientQuery, UserClientDto?>,
    IQueryHandler<GetCurrentClientQuery, UserClientDto?>,
    IQueryHandler<GetClientQuery, ClientDto?>,
    IQueryHandler<GetClientsQuery, List<ClientDto>>,
    IQueryHandler<GetPanelClientIdQuery, Guid?>
{
    private readonly IReadModel _readModel;

    public GetClientsQueryHandler(IReadModel readModel)
    {
        _readModel = readModel;
    }

    public async Task<List<UserClientDto>> HandleAsync(GetAccountClientsQuery query)
    {
        var userId = new UserId(query.UserId);
        var clients = await HubClients().Where(c => c.UserId == userId).ToListAsync();
        return clients.OrderBy(c => c.RegisteredAt).ThenBy(c => c.Hub!.Name).Select(c => c.ToAccountDto()).ToList();
    }

    public async Task<UserClientDto?> HandleAsync(GetAccountClientQuery query)
    {
        var client = await FindAsync(query.ClientId);
        return client != null && client.UserId.Value == query.UserId ? client.ToAccountDto() : null;
    }

    public async Task<UserClientDto?> HandleAsync(GetCurrentClientQuery query)
    {
        return (await FindAsync(query.ClientId))?.ToAccountDto();
    }

    public async Task<ClientDto?> HandleAsync(GetClientQuery query)
    {
        return (await FindAsync(query.ClientId))?.ToDto();
    }

    public async Task<List<ClientDto>> HandleAsync(GetClientsQuery query)
    {
        var clientIds = query.ClientIds.Select(id => new ClientId(id)).ToList();
        var clients = await HubClients().Where(c => clientIds.Contains(c.Id)).ToListAsync();
        return clients.Select(c => c.ToDto()).ToList();
    }

    public async Task<Guid?> HandleAsync(GetPanelClientIdQuery query)
    {
        var userId = new UserId(query.UserId);
        var panel = await _readModel.Query<Client>().FirstOrDefaultAsync(c => c.UserId == userId && c.Kind == ClientKind.Panel);
        return panel?.Id.Value;
    }

    private IQueryable<Client> HubClients()
    {
        return _readModel.Query<Client>().Where(c => c.Kind == ClientKind.Hub);
    }

    private async Task<Client?> FindAsync(Guid id)
    {
        var clientId = new ClientId(id);
        return await HubClients().FirstOrDefaultAsync(c => c.Id == clientId);
    }
}
