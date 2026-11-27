namespace Dara.Shared.Contracts.Clients;

// the whole context of a client; it goes to the model provider with prompts, so never secrets
public record ClientContextRequest(IReadOnlyDictionary<string, string> Entries);
