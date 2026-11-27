using Dara.Server.Modules.Identity.Domain.Clients;

namespace Dara.Server.Modules.Identity.Application.Clients;

// clients of the hub only: the panel client has no name, type or context and is never shown as a client
internal static class ClientMapping
{
    public static ClientDto ToDto(this Client client)
    {
        var hub = client.Hub!;
        return new ClientDto(client.Id.Value, client.UserId.Value, hub.Name, hub.Type.ToString(), client.IsRevoked, client.RegisteredAt);
    }

    public static UserClientDto ToAccountDto(this Client client)
    {
        var hub = client.Hub!;
        return new UserClientDto(
            client.Id.Value,
            client.UserId.Value,
            hub.Name,
            hub.Type.ToString(),
            client.IsRevoked,
            client.RegisteredAt,
            client.RevokedAt,
            hub.Context.ToDictionary());
    }

    public static Dictionary<string, string> ToDictionary(this ClientContext context)
    {
        return context.Entries.ToDictionary(e => e.Key, e => e.Value);
    }
}
