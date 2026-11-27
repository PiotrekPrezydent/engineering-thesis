namespace Dara.Server.Apps.API.Common.Seeding;

// sample data of the startup seed and ids created by earlier seeders
public class SeedData
{
    public IReadOnlyList<SeedAccount> Accounts { get; } =
    [
        new("bob"),
        new("room-101")
    ];

    // every client registers itself with a registration code of its account, so the server gives the ids
    public IReadOnlyList<SeedClient> Clients { get; } =
    [
        new("Bob-Phone", "bob", "Phone"),
        new("Bob-Laptop", "bob", "Pc"),
        new("Bob-Smartwatch", "bob", "Watch"),
        new("Room-101-Computer-01", "room-101", "Pc"),
        new("Room-101-Computer-02", "room-101", "Pc")
    ];

    // groups are owned and joined by clients (a member is a client)
    public IReadOnlyList<SeedGroup> Groups { get; } =
    [
        new("group-1", "group-1-jc", "Bob-Phone", ["Bob-Laptop", "Bob-Smartwatch", "Room-101-Computer-01", "Room-101-Computer-02"]),
        new("group-2", "group-2-jc", "Bob-Laptop", ["Bob-Smartwatch", "Room-101-Computer-01", "Room-101-Computer-02"]),
        new("group-3", "group-3-jc", "Bob-Smartwatch", ["Room-101-Computer-01", "Room-101-Computer-02"])
    ];

    // the login of the account whose user administers the server (Authorization.ServerAdmin)
    public string ServerAdmin { get; } = "bob";

    // account login -> user id, filled by the identity seeder
    public Dictionary<string, Guid> UserIds { get; } = new();

    // client name -> client id, filled by the identity seeder
    public Dictionary<string, Guid> ClientIds { get; } = new();

    // group name -> group id
    public Dictionary<string, Guid> GroupIds { get; } = new();
}

// the login of the account
public record SeedAccount(string Login)
{
    public string Email => $"{Login}@dara.local";
}

// Account is the login of the owner, ClientType the name of the type
public record SeedClient(string Name, string Account, string ClientType);

// Owner and Members are names of clients
public record SeedGroup(string Name, string JoinCode, string Owner, IReadOnlyList<string> Members);
