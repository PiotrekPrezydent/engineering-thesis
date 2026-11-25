using Google.GenAI;
using Microsoft.Extensions.AI;

namespace Dara.Server.Modules.Orchestration.Infrastructure.AgentRuns.Generation;

// gemini models through the google sdk, one client per plan generation
public class GoogleChatClientProvider : IChatClientProvider
{
    public const string Name = "google";

    public string ProviderName => Name;

    public IChatClient Create(string apiKey, string modelName)
    {
        return new Client(apiKey: apiKey).AsIChatClient(modelName);
    }
}
