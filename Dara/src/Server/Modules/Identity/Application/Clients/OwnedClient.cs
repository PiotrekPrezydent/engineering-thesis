using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Identity.Domain.Clients;
using Dara.Server.Modules.Identity.Domain.Clients.Rules;
using Dara.Server.Modules.Identity.Domain.Users;

namespace Dara.Server.Modules.Identity.Application.Clients;

// a change of a client from the panel of its account: a missing client and a client of another account both break
// ClientMustBelongToUserRule, so the panel cannot tell them apart
internal static class OwnedClient
{
    public static async Task<Client> GetAsync(this IClientRepository clients, Guid userId, Guid clientId)
    {
        var client = await clients.FindByIdAsync(new ClientId(clientId));
        if (client == null)
            throw new BuisnessRuleValidationException(new ClientMustBelongToUserRule(false));

        client.EnsureOwnedBy(new UserId(userId));
        return client;
    }
}
