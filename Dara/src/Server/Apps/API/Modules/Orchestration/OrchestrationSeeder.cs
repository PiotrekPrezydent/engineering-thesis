using Dara.Server.Modules.Orchestration.Application;
using Dara.Server.Modules.Orchestration.Application.Groups.EditApiKey;
using Dara.Server.Modules.Orchestration.Application.Groups.EditChatClientProviderName;
using Dara.Server.Modules.Orchestration.Application.Groups.EditClientModelName;
using Dara.Server.Modules.Orchestration.Application.Groups.GetGroupAgentSettings;

using Dara.Server.Apps.API.Common.Seeding;

namespace Dara.Server.Apps.API.Modules.Orchestration;

// the owner of every seeded group sets its agent settings, the api key comes from .env and is never logged
public class OrchestrationSeeder : ISeeder
{
    private readonly IOrchestrationModule _orchestrationModule;
    private readonly SeedEnvironment _environment;
    private readonly ILogger<OrchestrationSeeder> _logger;

    public OrchestrationSeeder(IOrchestrationModule orchestrationModule, SeedEnvironment environment, ILogger<OrchestrationSeeder> logger)
    {
        _orchestrationModule = orchestrationModule;
        _environment = environment;
        _logger = logger;
    }

    public async Task SeedAsync(SeedData data, CancellationToken cancellationToken)
    {
        foreach (var group in data.Groups)
        {
            var groupId = data.GroupIds[group.Name];
            var ownerId = data.ClientIds[group.Owner];

            // orchestration gets the group from the groups event asynchronously
            await _environment.WaitUntilAsync(async () =>
                await _orchestrationModule.ExecuteQueryAsync<GetGroupAgentSettingsQuery, GroupAgentSettingsDto?>(new GetGroupAgentSettingsQuery(groupId, ownerId)) is not null,
                $"orchestration group {group.Name}", cancellationToken);

            await _orchestrationModule.ExecuteCommandAsync(new EditChatClientProviderNameCommand(groupId, ownerId, _environment.AgentProvider));
            await _orchestrationModule.ExecuteCommandAsync(new EditClientModelNameCommand(groupId, ownerId, _environment.AgentModel));
            await _orchestrationModule.ExecuteCommandAsync(new EditApiKeyCommand(groupId, ownerId, _environment.AgentApiKey));
        }

        _logger.LogInformation("Seeded agent settings of {Count} groups ({Provider}, {Model}, api key {KeyState})",
            data.Groups.Count, _environment.AgentProvider, _environment.AgentModel,
            string.IsNullOrEmpty(_environment.AgentApiKey) ? "empty" : "set");
    }
}
