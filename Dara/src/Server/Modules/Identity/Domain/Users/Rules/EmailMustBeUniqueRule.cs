using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Users.Rules;

public class EmailMustBeUniqueRule : IBuisnessRule
{
    private readonly string _normalizedEmail;
    private readonly IUserUniquenessChecker _checker;

    public EmailMustBeUniqueRule(string normalizedEmail, IUserUniquenessChecker checker)
    {
        _normalizedEmail = normalizedEmail;
        _checker = checker;
    }

    public string Message => "E-mail is already used by another account";

    public bool IsBroken()
    {
        return _checker.IsEmailTaken(_normalizedEmail);
    }
}
