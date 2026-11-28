using System.Security.Claims;
using Dara.Server.Apps.API.Common.Authentication;
using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.Clients;
using Dara.Server.Modules.Identity.Application.GetClients;
using Dara.Server.Modules.Identity.Domain.Clients.Rules;

namespace Dara.Server.Apps.API.Common.Authorization;

// the hub client a request acts for: a client token acts for itself, the panel of an account for a hub client of
// that account (ViaPanel); modules get only ClientId and never learn which way the request came
public sealed record ActingClient(Guid ClientId, Guid UserId, bool ViaPanel);

// the same check as a change of a client in the panel, made by the host before a command of another module, so
// groups, plugins and orchestration need not know which client belongs to which account
public class ActingClientResolver
{
    private readonly IIdentityModule _identity;

    public ActingClientResolver(IIdentityModule identity)
    {
        _identity = identity;
    }

    // a revoked client is refused by the jwt check already
    public static ActingClient FromToken(ClaimsPrincipal user)
    {
        return new ActingClient(user.ClientId(), user.UserId(), false);
    }

    // a missing client, a client of another account and the panel client: 404; a revoked one: 409
    public async Task<ActingClient> FromPanelAsync(ClaimsPrincipal user, Guid clientId)
    {
        var client = await _identity.ExecuteQueryAsync<GetAccountClientQuery, UserClientDto?>(new GetAccountClientQuery(user.UserId(), clientId));
        if (client is null)
            throw new BuisnessRuleValidationException(new ClientMustBelongToUserRule(false));
        if (client.IsRevoked)
            throw new BuisnessRuleValidationException(new ClientMustNotBeRevokedRule(true));

        return new ActingClient(client.ClientId, client.UserId, true);
    }
}
