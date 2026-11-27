namespace Dara.Server.Modules.Identity.Domain.Users.Clients;

// sent by the app of the client when it registers; plugins use it for compatible client types
public enum ClientType
{
    Pc,
    Phone,
    Tablet,
    Watch,
    Other
}