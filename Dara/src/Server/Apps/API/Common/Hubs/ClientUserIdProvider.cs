using Dara.Shared.Contracts.Auth;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Common.Hubs;

// Clients.User(clientId) addresses every connection of one client: the hub takes client tokens only, and the id of a
// client is the id of the member, the plugin owner and the participant
public class ClientUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(AuthClaims.Client)?.Value;
    }
}
