using Dara.Shared.Contracts.Auth;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Authentication;

// Clients.User(userId) addresses every connection of the account; TASK-ENG-029 switches it to the device claim
public class SubjectUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(AuthClaims.Subject)?.Value;
    }
}
