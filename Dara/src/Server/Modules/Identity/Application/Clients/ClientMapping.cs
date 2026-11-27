using Dara.Server.Modules.Identity.Domain.Users.Clients;

namespace Dara.Server.Modules.Identity.Application.Clients;

internal static class ClientMapping
{
    public static ClientDto ToDto(this UserClient client)
    {
        return new ClientDto(client.Id.Value, client.UserId.Value, client.Name, client.Type.ToString(), client.IsRevoked, client.RegisteredAt);
    }

    public static UserClientDto ToAccountDto(this UserClient client)
    {
        return new UserClientDto(
            client.Id.Value,
            client.UserId.Value,
            client.Name,
            client.Type.ToString(),
            client.IsRevoked,
            client.RegisteredAt,
            client.RevokedAt,
            client.Context.ToDictionary());
    }

    public static Dictionary<string, string> ToDictionary(this ClientContext context)
    {
        return context.Entries.ToDictionary(e => e.Key, e => e.Value);
    }
}
