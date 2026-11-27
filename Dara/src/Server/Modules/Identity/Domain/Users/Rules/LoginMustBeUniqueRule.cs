using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Users.Rules;

public class LoginMustBeUniqueRule : IBuisnessRule
{
    private readonly string _normalizedLogin;
    private readonly IUserUniquenessChecker _checker;

    public LoginMustBeUniqueRule(string normalizedLogin, IUserUniquenessChecker checker)
    {
        _normalizedLogin = normalizedLogin;
        _checker = checker;
    }

    public string Message => "Login is already taken";

    public bool IsBroken()
    {
        return _checker.IsLoginTaken(_normalizedLogin);
    }
}
