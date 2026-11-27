using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Identity.Domain.Users.ClientRegistrationCodes;
using Dara.Server.Modules.Identity.Domain.Users.Rules;

namespace Dara.Server.Modules.Identity.Domain.Users;

// the account: its id is the user id that other modules know; the password is kept only as a slow salted hash; it is
// not a subject of Authorization: its clients are (Client, the panel client too), each with its own roles; the
// account keeps the open registration codes, because a code says which account a new client joins
public class User : Entity, IAggregateRoot
{
    private readonly List<ClientRegistrationCode> _clientRegistrationCodes = new();

    public UserId Id { get; private set; }

    public string Login { get; private set; }
    public string Email { get; private set; }
    public string NormalizedEmail { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime RegisteredAt { get; private set; }

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
    }

    // the application creates the panel client of the account in the same unit of work (Client.CreatePanel), which
    // publishes the new account
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

    // uses up the code; false when it is unknown, used or expired (a result, not an exception, like every refused
    // sign-in); the application registers the new client (Client.RegisterHub) and starts its session in the same unit
    // of work, so a code is used once
    public bool UseClientRegistrationCode(string registrationCodeHash, DateTime now)
    {
        var code = _clientRegistrationCodes.FirstOrDefault(c => c.Hash == registrationCodeHash);
        if (code is null || code.IsExpired(now))
            return false;

        _clientRegistrationCodes.Remove(code);
        return true;
    }

    public static string NormalizeLogin(string? login)
    {
        return (login ?? string.Empty).Trim().ToLowerInvariant();
    }

    public static string NormalizeEmail(string? email)
    {
        return (email ?? string.Empty).Trim().ToUpperInvariant();
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
