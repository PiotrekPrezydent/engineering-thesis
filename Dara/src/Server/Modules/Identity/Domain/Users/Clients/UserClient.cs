using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Identity.Domain.Users.Rules;

namespace Dara.Server.Modules.Identity.Domain.Users.Clients;

// one app signed in with the account (the CLI on a computer, the phone app); it belongs to one account for good and
// changes only through User, which first checks the owner; a revoked client takes no change at all
public class UserClient : Entity
{
    public UserClientId Id { get; private set; }
    public UserId UserId { get; private set; }
    public string Name { get; private set; }
    public ClientType Type { get; private set; }
    public ClientContext Context { get; private set; }
    public DateTime RegisteredAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    // the open code of the panel, null when none was issued or it was used
    public ClientLoginCode? LoginCode { get; private set; }

    public bool IsRevoked => RevokedAt != null;

    private UserClient() { }

    private UserClient(UserId userId, string name, ClientType type, DateTime now)
    {
        Id = new UserClientId(Guid.NewGuid());
        UserId = userId;
        Name = name;
        Type = type;
        Context = new ClientContext([]);
        RegisteredAt = now;
    }

    // the name is checked by User, which may give a default one
    internal static UserClient Create(UserId userId, string name, ClientType type, DateTime now)
    {
        return new UserClient(userId, name, type, now);
    }

    internal ClientLoginCode IssueLoginCode(string codeHash, DateTime now, TimeSpan lifetime)
    {
        CheckRule(new ClientMustNotBeRevokedRule(IsRevoked));

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
        CheckRule(new ClientMustNotBeRevokedRule(IsRevoked));

        if (!HasLoginCode(codeHash) || LoginCode!.IsExpired(now))
            return false;

        LoginCode = null;
        return true;
    }

    // false when the name stays the same
    internal bool Rename(string name)
    {
        CheckRule(new ClientMustNotBeRevokedRule(IsRevoked));
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
        CheckRule(new ClientMustNotBeRevokedRule(IsRevoked));
        CheckRule(new ClientContextMustBeValidRule(entries));

        var newEntries = entries.Select(e => new ClientContextEntry(e.Key.Trim(), e.Value.Trim())).ToList();
        if (newEntries.Count == Context.Entries.Count && newEntries.All(Context.Entries.Contains))
            return false;

        Context = new ClientContext(newEntries);
        return true;
    }

    internal void Revoke(DateTime now)
    {
        CheckRule(new ClientMustNotBeRevokedRule(IsRevoked));

        RevokedAt = now;
        LoginCode = null;
    }
}
