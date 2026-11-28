using Dara.Shared.Contracts.Auth;

namespace Dara.Shared.Contracts.Groups;

// http endpoints of groups: /groups/* with the client token (the client writes), /account/groups/* and
// /account/clients/{clientId}/groups/* with the cookie of the account panel (the panel manages)
public static class GroupRoutes
{
    public const string ClientGroups = "/groups";
    public const string AccountGroups = "/account/groups";

    public static string Messages(Guid groupId) => $"{ClientGroups}/{groupId}/messages";

    public static string ClientGroupsOf(Guid clientId) => $"{AuthRoutes.AccountClientsGroup}/{clientId}/groups";
    public static string Join(Guid clientId) => $"{ClientGroupsOf(clientId)}/join";
    public static string Membership(Guid clientId, Guid groupId) => $"{ClientGroupsOf(clientId)}/{groupId}";
}
