using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Identity.Domain;
using Dara.Server.Modules.Identity.Domain.Events;
using Dara.Server.Modules.Identity.Domain.Rules;

namespace Dara.Server.Modules.Identity.UnitTests;

public class UserTests
{
    private const string Password = "password-1";
    private static readonly DateTime Now = new(2026, 11, 26, 12, 0, 0, DateTimeKind.Utc);

    private readonly FakePasswordHasher _hasher = new();
    private readonly FakeUniquenessChecker _checker = new();

    [Fact]
    public void RegisterNormalizesTheLoginAndHashesThePassword()
    {
        var user = User.Register(" Alice ", " alice@test.local ", Password, Now, _hasher, _checker);

        Assert.Equal("alice", user.Login);
        Assert.Equal("alice@test.local", user.Email);
        Assert.Equal("ALICE@TEST.LOCAL", user.NormalizedEmail);
        Assert.Equal("hash:" + Password, user.PasswordHash);
        Assert.Equal(Now, user.RegisteredAt);
        Assert.IsType<NewUserCreatedDomainEvent>(Assert.Single(user.DomainEvents));
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("a b c")]
    [InlineData("alice!")]
    [InlineData("a23456789012345678901234567890123")]
    public void RegisterRefusesALoginOfWrongFormat(string login)
    {
        AssertBroken<LoginMustHaveValidFormatRule>(() => User.Register(login, "a@test.local", Password, Now, _hasher, _checker));
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    [InlineData("Alice <alice@test.local>")]
    public void RegisterRefusesAnEMailOfWrongFormat(string email)
    {
        AssertBroken<EmailMustHaveValidFormatRule>(() => User.Register("alice", email, Password, Now, _hasher, _checker));
    }

    [Fact]
    public void RegisterRefusesATooLongEMail()
    {
        var email = new string('a', 250) + "@t.pl";
        AssertBroken<EmailMustHaveValidFormatRule>(() => User.Register("alice", email, Password, Now, _hasher, _checker));
    }

    [Theory]
    [InlineData(7)]
    [InlineData(129)]
    public void RegisterRefusesAPasswordOutsideThePolicy(int length)
    {
        AssertBroken<PasswordMustMeetPolicyRule>(() => User.Register("alice", "a@test.local", new string('p', length), Now, _hasher, _checker));
    }

    [Fact]
    public void RegisterRefusesATakenLoginAndATakenEMail()
    {
        _checker.Logins.Add("alice");
        _checker.Emails.Add("BOB@TEST.LOCAL");

        AssertBroken<LoginMustBeUniqueRule>(() => User.Register("ALICE", "a@test.local", Password, Now, _hasher, _checker));
        AssertBroken<EmailMustBeUniqueRule>(() => User.Register("bob", "Bob@Test.local", Password, Now, _hasher, _checker));
    }

    [Fact]
    public void VerifyPasswordReplacesAnOldHash()
    {
        var user = User.Register("alice", "a@test.local", Password, Now, _hasher, _checker);
        typeof(User).GetProperty(nameof(User.PasswordHash))!.SetValue(user, "old:" + Password);

        Assert.Equal(PasswordCheck.Failed, user.VerifyPassword("wrong-password", _hasher));
        Assert.Equal("old:" + Password, user.PasswordHash);

        Assert.Equal(PasswordCheck.SuccessRehashNeeded, user.VerifyPassword(Password, _hasher));
        Assert.Equal("hash:" + Password, user.PasswordHash);
        Assert.Equal(PasswordCheck.Success, user.VerifyPassword(Password, _hasher));
    }

    private static void AssertBroken<TRule>(Action action)
    {
        var exception = Assert.Throws<BuisnessRuleValidationException>(action);
        Assert.IsType<TRule>(exception.BrokenRule);
    }
}
