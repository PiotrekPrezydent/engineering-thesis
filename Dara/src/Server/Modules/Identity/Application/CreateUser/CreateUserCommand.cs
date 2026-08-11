using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Identity.Application.CreateUser;

public record CreateUserCommand(string UserIdentifier) : ICommand<Guid>;