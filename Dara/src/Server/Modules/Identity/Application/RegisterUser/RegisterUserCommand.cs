using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Identity.Application.RegisterUser;

public record RegisterUserCommand(string Login, string Email, string Password) : ICommand<Guid>;
