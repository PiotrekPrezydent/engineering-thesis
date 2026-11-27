using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Clients;

// one entry of the context of a client ("Photos folder" = "D:\Photos"): orchestration adds the context of the
// executor to the prompt, so everything here goes to the model provider and must hold no secrets
public record ClientContextEntry(string Key, string Value) : IValueObject;