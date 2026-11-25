using Dara.Server.Modules.Orchestration.Infrastructure.AgentRuns.Generation;
using Microsoft.Extensions.AI;

namespace Dara.Server.Modules.Orchestration.IntegrationTests.Fakes;

// stands in for the google provider, every plan generation gets the scripted client of the test
public class ScriptedChatClientProvider : IChatClientProvider
{
    private readonly Lock _lock = new();
    private readonly List<(string ApiKey, string ModelName)> _clients = new();

    public ScriptedChatClientProvider(params ScriptedRound[] rounds)
    {
        Client = new ScriptedPlanningChatClient(rounds);
    }

    public string ProviderName => GoogleChatClientProvider.Name;

    public ScriptedPlanningChatClient Client { get; private set; }

    public IReadOnlyList<(string ApiKey, string ModelName)> CreatedClients
    {
        get
        {
            lock (_lock)
                return _clients.ToList();
        }
    }

    // the next generation answers with these rounds
    public void Script(params ScriptedRound[] rounds)
    {
        Client = new ScriptedPlanningChatClient(rounds);
    }

    public IChatClient Create(string apiKey, string modelName)
    {
        lock (_lock)
            _clients.Add((apiKey, modelName));

        return Client;
    }
}
