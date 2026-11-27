using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Identity.Application.Clients;
using Dara.Server.Modules.Identity.Domain.Users;
using Dara.Server.Modules.Identity.Domain.Users.Clients;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Identity.Application.GetClients;

// clients are entities of User, so every query reads the accounts that have them
public class GetClientsQueryHandler :
    IQueryHandler<GetAccountClientsQuery, List<UserClientDto>>,
    IQueryHandler<GetAccountClientQuery, UserClientDto?>,
    IQueryHandler<GetCurrentClientQuery, UserClientDto?>,
    IQueryHandler<GetClientQuery, ClientDto?>,
    IQueryHandler<GetClientsQuery, List<ClientDto>>
{
    private readonly IReadModel _readModel;

    public GetClientsQueryHandler(IReadModel readModel)
    {
        _readModel = readModel;
    }

    public async Task<List<UserClientDto>> HandleAsync(GetAccountClientsQuery query)
    {
        var userId = new UserId(query.UserId);
        var user = await _readModel.Query<User>().FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return [];

        return user.Clients.OrderBy(c => c.RegisteredAt).ThenBy(c => c.Name).Select(c => c.ToAccountDto()).ToList();
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
        var clientIds = query.ClientIds.Select(id => new UserClientId(id)).ToList();
        var users = await _readModel.Query<User>().Where(u => u.Clients.Any(c => clientIds.Contains(c.Id))).ToListAsync();
        return users.SelectMany(u => u.Clients).Where(c => clientIds.Contains(c.Id)).Select(c => c.ToDto()).ToList();
    }

    private async Task<UserClient?> FindAsync(Guid id)
    {
        var clientId = new UserClientId(id);
        var user = await _readModel.Query<User>().FirstOrDefaultAsync(u => u.Clients.Any(c => c.Id == clientId));
        return user?.Clients.First(c => c.Id == clientId);
    }
}
