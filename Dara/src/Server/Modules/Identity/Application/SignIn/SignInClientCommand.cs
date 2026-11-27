using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Identity.Application.SignIn;

// an existing client signs in only with the one-time code the account panel issued for it, never with the password
public record SignInClientCommand(string Code) : ICommand<ClientTokensResult>;
