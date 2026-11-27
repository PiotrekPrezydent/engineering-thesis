namespace Dara.Server.Modules.Identity.Domain.Clients;

// a client of the hub is an app signed in with the account (rpc over SignalR, refresh tokens); the panel client is the
// account panel over http, one per account, created with the account; both are subjects with their own roles
public enum ClientKind
{
    Hub,
    Panel
}
