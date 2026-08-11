using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Identity.Application.CreateClientIdentity;

public record CreateClientIdentityCommand(string ClientIdentifier) : ICommand<Guid>;