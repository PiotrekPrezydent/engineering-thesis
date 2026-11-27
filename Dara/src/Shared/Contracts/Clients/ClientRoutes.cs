using Dara.Shared.Contracts.Auth;

namespace Dara.Shared.Contracts.Clients;

// http endpoints of clients: /account/clients/* with the cookie of the account panel, /clients/current with the
// client token; the registration code and the login code are in AuthRoutes
public static class ClientRoutes
{
    public const string AccountClients = AuthRoutes.AccountClientsGroup;
    public const string Current = "/clients/current";

    public static string Client(Guid clientId) => $"{AccountClients}/{clientId}";
    public static string Name(Guid clientId) => $"{Client(clientId)}/name";
    public static string Context(Guid clientId) => $"{Client(clientId)}/context";
    public static string Revoke(Guid clientId) => $"{Client(clientId)}/revoke";
}
