using Dara.Server.Modules.Orchestration.Application;
using Dara.Server.Modules.Orchestration.Application.Participants.GetParticipantFunctions;
using Dara.Server.Modules.Plugins.Application;
using Dara.Server.Modules.Plugins.Application.Plugins.RegisterPlugins;

namespace Dara.Server.Apps.API.Services.Seeding;

public class PluginsSeeder : ISeeder
{
    private readonly IPluginsModule _pluginsModule;
    private readonly IOrchestrationModule _orchestrationModule;
    private readonly SeedEnvironment _environment;
    private readonly ILogger<PluginsSeeder> _logger;

    public PluginsSeeder(IPluginsModule pluginsModule, IOrchestrationModule orchestrationModule, SeedEnvironment environment, ILogger<PluginsSeeder> logger)
    {
        _pluginsModule = pluginsModule;
        _orchestrationModule = orchestrationModule;
        _environment = environment;
        _logger = logger;
    }

    public async Task SeedAsync(SeedData data, CancellationToken cancellationToken)
    {
        // registered like a client after connecting, the plugin owner may not be projected yet
        foreach (var (identifier, manifests) in SampleManifests.ByUser)
            await _pluginsModule.ExecuteCommandAsync(new RegisterPluginsCommand(data.UserIds[identifier], manifests));

        // participants get the functions from the plugins event asynchronously
        var expected = SampleManifests.ByUser.ToDictionary(e => data.UserIds[e.Key], e => e.Value.Sum(m => m.Functions.Count));
        await _environment.WaitUntilAsync(async () =>
        {
            foreach (var (userId, count) in expected)
            {
                var functions = await _orchestrationModule.ExecuteQueryAsync<GetParticipantFunctionsQuery, List<ParticipantFunctionDto>>(new GetParticipantFunctionsQuery(userId));
                if (functions.Count != count)
                    return false;
            }

            return true;
        }, "plugin functions of seeded participants", cancellationToken);

        _logger.LogInformation("Seeded plugins of {Count} users", expected.Count);
    }
}
