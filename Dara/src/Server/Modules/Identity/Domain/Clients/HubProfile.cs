using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Identity.Domain.Clients.Rules;

namespace Dara.Server.Modules.Identity.Domain.Clients;

// what only a client of the hub has: its name, type, context and the open login code of the panel; it changes only
// through its Client, which first checks the kind and the revocation
public class HubProfile : Entity
{
    public string Name { get; private set; }
    public ClientType Type { get; private set; }
    public ClientContext Context { get; private set; }
    // the open code of the panel, null when none was issued or it was used
    public ClientLoginCode? LoginCode { get; private set; }

    private HubProfile() { }

    private HubProfile(string name, ClientType type)
    {
        Name = name;
        Type = type;
        Context = new ClientContext([]);
    }

    // the name is checked by Client, which may give a default one
    internal static HubProfile Create(string name, ClientType type)
    {
        return new HubProfile(name, type);
    }

    internal ClientLoginCode IssueLoginCode(string codeHash, DateTime now, TimeSpan lifetime)
    {
        LoginCode = ClientLoginCode.Create(codeHash, now, lifetime);
        return LoginCode;
    }

    internal bool HasLoginCode(string codeHash)
    {
        return LoginCode is not null && LoginCode.Hash == codeHash;
    }

    // the code is used once; false when it is not the open code or it expired (an expired code stays until replaced)
    internal bool ConsumeLoginCode(string codeHash, DateTime now)
    {
        if (!HasLoginCode(codeHash) || LoginCode!.IsExpired(now))
            return false;

        LoginCode = null;
        return true;
    }

    // false when the name stays the same
    internal bool Rename(string name)
    {
        CheckRule(new ClientNameMustBeValidRule(name));

        var newName = name.Trim();
        if (newName == Name)
            return false;

        Name = newName;
        return true;
    }

    // keys and values are kept trimmed; false when the context stays the same
    internal bool ChangeContext(IReadOnlyDictionary<string, string> entries)
    {
        CheckRule(new ClientContextMustBeValidRule(entries));

        var newEntries = entries.Select(e => new ClientContextEntry(e.Key.Trim(), e.Value.Trim())).ToList();
        if (newEntries.Count == Context.Entries.Count && newEntries.All(Context.Entries.Contains))
            return false;

        Context = new ClientContext(newEntries);
        return true;
    }

    internal void DropLoginCode()
    {
        LoginCode = null;
    }
}
