namespace Dara.Server.Modules.Identity.Domain.Users.Clients;

// labels of client types for the name a client gets when it registers without a name ("PC 1", "Phone 2")
public static class ClientTypeLabels
{
    public static string Label(ClientType type) => type switch
    {
        ClientType.Pc => "PC",
        ClientType.Phone => "Phone",
        ClientType.Tablet => "Tablet",
        ClientType.Watch => "Watch",
        _ => "Client"
    };

    // the number counts the clients of this type in the account, revoked ones too, so a name is never reused
    public static string DefaultName(ClientType type, int sameTypeCount) => $"{Label(type)} {sameTypeCount + 1}";

    // the type as text from the app, without regard to case; unknown, empty or numeric text is Other
    public static ClientType Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || char.IsDigit(value.Trim()[0]))
            return ClientType.Other;

        return Enum.TryParse<ClientType>(value.Trim(), ignoreCase: true, out var type) && Enum.IsDefined(type)
            ? type
            : ClientType.Other;
    }
}
