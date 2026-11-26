namespace Dara.Server.Apps.API.Services.Seeding;

// sample data of the startup seed and ids created by earlier seeders
public class SeedData
{
    public IReadOnlyList<SeedUser> Users { get; } =
    [
        new("user-1", "Bob-Phone"),
        new("user-2", "Bob-Laptop"),
        new("user-3", "Bob-Smartwatch"),
        new("user-4", "Room-101-Computer-01"),
        new("user-5", "Room-101-Computer-02")
    ];

    public IReadOnlyList<SeedGroup> Groups { get; } =
    [
        new("group-1", "group-1-jc", "user-1", ["user-2", "user-3", "user-4", "user-5"]),
        new("group-2", "group-2-jc", "user-2", ["user-3", "user-4", "user-5"]),
        new("group-3", "group-3-jc", "user-3", ["user-4", "user-5"])
    ];

    // user identifier -> user id
    public Dictionary<string, Guid> UserIds { get; } = new();

    // group name -> group id
    public Dictionary<string, Guid> GroupIds { get; } = new();
}

// the identifier is the login of the account
public record SeedUser(string Identifier, string ProfileName)
{
    public string Email => $"{Identifier}@dara.local";
}

public record SeedGroup(string Name, string JoinCode, string Owner, IReadOnlyList<string> Members);
