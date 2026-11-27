using Dara.Server.Modules.Identity.Application.Clients;
using Dara.Shared.Contracts.Clients;

namespace Dara.Server.Apps.API.Modules.Identity;

// dtos of the identity module in the shape of the client contract
public static class ClientContractMapping
{
    public static AccountClientSnapshot ToSnapshot(this UserClientDto client)
    {
        return new AccountClientSnapshot(
            client.ClientId,
            client.UserId,
            client.Name,
            client.ClientType,
            client.IsRevoked,
            client.RegisteredAt,
            client.RevokedAt,
            client.Context);
    }
}
