using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Identity.Application.SignIn;

// a sign-in of the account panel with the password; the host keeps the session id in a cookie
public record SignInAccountCommand(string Login, string Password) : ICommand<AccountSignInResult>;
