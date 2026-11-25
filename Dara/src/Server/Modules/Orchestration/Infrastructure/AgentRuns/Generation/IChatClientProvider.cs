using Microsoft.Extensions.AI;

namespace Dara.Server.Modules.Orchestration.Infrastructure.AgentRuns.Generation;

// creates the chat client of one model provider, the name is the one kept in the group settings
public interface IChatClientProvider
{
    string ProviderName { get; }

    IChatClient Create(string apiKey, string modelName);
}
