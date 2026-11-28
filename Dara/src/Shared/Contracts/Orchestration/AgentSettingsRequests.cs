namespace Dara.Shared.Contracts.Orchestration;

// one setting per request: one command each, a joint change would not be atomic
public record EditPromptMessagePrefixRequest(string PromptMessagePrefix);

public record EditChatClientProviderRequest(string ChatClientProviderName);

public record EditClientModelRequest(string ClientModelName);

public record EditApiKeyRequest(string ApiKey);
