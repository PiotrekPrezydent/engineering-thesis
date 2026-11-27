using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Clients;

// the whole context of a client, kept in one json column and changed only as a whole
public record ClientContext(IReadOnlyList<ClientContextEntry> Entries) : IValueObject;