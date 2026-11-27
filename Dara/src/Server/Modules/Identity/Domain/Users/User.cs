using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Identity.Domain.Users.ClientRegistrationCodes;
using Dara.Server.Modules.Identity.Domain.Users.Clients;
using Dara.Server.Modules.Identity.Domain.Users.Events;
using Dara.Server.Modules.Identity.Domain.Users.Rules;

namespace Dara.Server.Modules.Identity.Domain.Users;

// the account: its id is the user id that other modules know; the password is kept only as a slow salted hash;
// the clients of the account (apps signed in with it) are entities of this aggregate and change only through the
// public methods here, each of which first checks that the client belongs to the account
public class User : Entity, IAggregateRoot
{
    private readonly List<UserClient> _clients = new();
    private readonly List<ClientRegistrationCode> _clientRegistrationCodes = new();

    public UserId Id { get; private set; }

    public string Login { get; private set; }
    public string Email { get; private set; }
    public string NormalizedEmail { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime RegisteredAt { get; private set; }

    // revoked clients stay: they keep the numbering of default names and take no change
    public IReadOnlyList<UserClient> Clients => _clients.AsReadOnly();

    // open codes only: a used code is removed at once, an expired one when the next code is created
    public IReadOnlyList<ClientRegistrationCode> ClientRegistrationCodes => _clientRegistrationCodes.AsReadOnly();

    private User() { }

    private User(string login, string email, string passwordHash, DateTime registeredAt)
    {
        Id = new UserId(Guid.NewGuid());
        Login = login;
        Email = email;
        NormalizedEmail = NormalizeEmail(email);
        PasswordHash = passwordHash;
        RegisteredAt = registeredAt;

        AddDomainEvent(new NewUserCreatedDomainEvent(Id));
    }

    public static User Register(string login, string email, string password, DateTime now, IPasswordHasher hasher, IUserUniquenessChecker checker)
    {
        var normalizedLogin = NormalizeLogin(login);
        var trimmedEmail = (email ?? string.Empty).Trim();

        // format first, so the uniqueness checks never query with an invalid value
        CheckRules(
            new LoginMustHaveValidFormatRule(normalizedLogin),
            new EmailMustHaveValidFormatRule(trimmedEmail),
            new PasswordMustMeetPolicyRule(password),
            new LoginMustBeUniqueRule(normalizedLogin, checker),
            new EmailMustBeUniqueRule(NormalizeEmail(trimmedEmail), checker));

        return new User(normalizedLogin, trimmedEmail, hasher.Hash(password), now);
    }

    // a hash made with older settings of the hasher is replaced by a new one on a successful check
    public PasswordCheck VerifyPassword(string password, IPasswordHasher hasher)
    {
        var check = hasher.Verify(PasswordHash, password);
        if (check == PasswordCheck.SuccessRehashNeeded)
            PasswordHash = hasher.Hash(password);

        return check;
    }

    // the panel shows the code once; the app of a new client registers itself with it, so the client never needs the
    // user id; several codes may be open at once, expired ones are removed here
    public ClientRegistrationCode CreateClientRegistrationCode(string codeHash, DateTime now, TimeSpan lifetime)
    {
        _clientRegistrationCodes.RemoveAll(c => c.IsExpired(now));

        var code = ClientRegistrationCode.Create(codeHash, now, lifetime);
        _clientRegistrationCodes.Add(code);
        return code;
    }

    // uses up the code and adds the client; null when the code is unknown, used or expired (a result, not an
    // exception, like every refused sign-in); an empty name gets the label of the type with a number; the
    // application starts the session of the new client in the same unit of work
    public UserClient? RegisterClient(string registrationCodeHash, ClientType type, string? name, DateTime now)
    {
        var code = _clientRegistrationCodes.FirstOrDefault(c => c.Hash == registrationCodeHash);
        if (code is null || code.IsExpired(now))
            return null;

        var sameTypeCount = _clients.Count(c => c.Type == type);
        var clientName = string.IsNullOrWhiteSpace(name) ? ClientTypeLabels.DefaultName(type, sameTypeCount) : name.Trim();
        CheckRule(new ClientNameMustBeValidRule(clientName));

        _clientRegistrationCodes.Remove(code);
        var client = UserClient.Create(Id, clientName, type, now);
        _clients.Add(client);

        AddDomainEvent(new UserClientRegisteredDomainEvent(Id, client.Id, client.Name, client.Type));
        return client;
    }

    // for an existing client only (a copy of the app on another computer); a new code replaces the previous one
    public ClientLoginCode IssueClientLoginCode(UserClientId clientId, string codeHash, DateTime now, TimeSpan lifetime)
    {
        return GetClient(clientId).IssueLoginCode(codeHash, now, lifetime);
    }

    // uses up the code; the id of the client whose code it was, null when no client has it or it expired; the
    // application then signs in the session of that client in the same unit of work
    public UserClientId? ConsumeClientLoginCode(string codeHash, DateTime now)
    {
        var client = _clients.FirstOrDefault(c => c.HasLoginCode(codeHash));
        if (client is null)
            return null;

        return client.ConsumeLoginCode(codeHash, now) ? client.Id : null;
    }

    public void RenameClient(UserClientId clientId, string name)
    {
        var client = GetClient(clientId);
        if (!client.Rename(name))
            return;

        AddDomainEvent(new UserClientRenamedDomainEvent(Id, client.Id, client.Name));
    }

    // replaces the whole context of the client
    public void ChangeClientContext(UserClientId clientId, IReadOnlyDictionary<string, string> entries)
    {
        var client = GetClient(clientId);
        if (!client.ChangeContext(entries))
            return;

        AddDomainEvent(new UserClientContextChangedDomainEvent(Id, client.Id, client.Context));
    }

    // the application ends the session of the client in the same unit of work
    public void RevokeClient(UserClientId clientId, DateTime now)
    {
        var client = GetClient(clientId);
        client.Revoke(now);

        AddDomainEvent(new UserClientRevokedDomainEvent(Id, client.Id));
    }

    public static string NormalizeLogin(string? login)
    {
        return (login ?? string.Empty).Trim().ToLowerInvariant();
    }

    public static string NormalizeEmail(string? email)
    {
        return (email ?? string.Empty).Trim().ToUpperInvariant();
    }

    // every change of a client goes through here: an id of a client of another account breaks the rule
    private UserClient GetClient(UserClientId clientId)
    {
        var client = _clients.FirstOrDefault(c => c.Id == clientId);
        CheckRule(new ClientMustBelongToUserRule(client is not null));
        return client!;
    }

    // the user does not exist yet when its rules are checked, so the instance method of Entity does not fit
    private static void CheckRules(params IBuisnessRule[] rules)
    {
        foreach (var rule in rules)
        {
            if (rule.IsBroken())
                throw new BuisnessRuleValidationException(rule);
        }
    }
}
