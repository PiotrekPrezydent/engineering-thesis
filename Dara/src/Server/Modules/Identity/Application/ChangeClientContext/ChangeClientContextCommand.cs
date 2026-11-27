using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Identity.Application.ChangeClientContext;

// replaces the whole context of the client; everything in it goes to the model provider, so no secrets
public record ChangeClientContextCommand(Guid UserId, Guid ClientId, IReadOnlyDictionary<string, string> Entries) : ICommand;
