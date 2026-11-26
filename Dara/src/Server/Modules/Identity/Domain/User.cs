using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Identity.Domain.Events;
using Dara.Server.Modules.Identity.Domain.Rules;

namespace Dara.Server.Modules.Identity.Domain;

// the account: its id is the user id that other modules know; the password is kept only as a slow salted hash
public class User : Entity, IAggregateRoot
{
    public UserId Id { get; private set; }

    public string Login { get; private set; }
    public string Email { get; private set; }
    public string NormalizedEmail { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime RegisteredAt { get; private set; }

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
