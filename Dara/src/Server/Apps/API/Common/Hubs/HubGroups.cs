namespace Dara.Server.Apps.API.Common.Hubs;

// signalr groups of the hub: every connection of one client and every connection of the clients of one account
// (TASK-ENG-032-D04); notifications go to these groups, never to Clients.User
public static class HubGroups
{
    public static string Client(Guid clientId) => $"client:{clientId}";

    public static string User(Guid userId) => $"user:{userId}";

    public static IReadOnlyList<string> Clients(IEnumerable<Guid> clientIds) => clientIds.Select(Client).ToList();
}
