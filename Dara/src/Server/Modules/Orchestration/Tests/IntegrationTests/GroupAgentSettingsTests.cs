using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Orchestration.Application.Groups.EditApiKey;
using Dara.Server.Modules.Orchestration.Application.Groups.EditChatClientProviderName;
using Dara.Server.Modules.Orchestration.Application.Groups.EditClientModelName;
using Dara.Server.Modules.Orchestration.Application.Groups.EditPromptMessagePrefix;
using Dara.Server.Modules.Orchestration.Application.Groups.GetGroupAgentSettings;
using Dara.Server.Modules.Orchestration.Domain.Groups.Rules;

namespace Dara.Server.Modules.Orchestration.IntegrationTests;

public class GroupAgentSettingsTests
{
    [Fact]
    public async Task OwnerEditsTheSettingsAndReadsTheKey()
    {
        await using var host = await OrchestrationModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var scenario = await PlanningScenario.CreateAsync(host, withSettings: false);

        await host.ExecuteAsync(new EditPromptMessagePrefixCommand(scenario.GroupId, scenario.PhoneId, "/ai"));
        await host.ExecuteAsync(new EditChatClientProviderNameCommand(scenario.GroupId, scenario.PhoneId, "google"));
        await host.ExecuteAsync(new EditClientModelNameCommand(scenario.GroupId, scenario.PhoneId, PlanningScenario.ModelName));
        await host.ExecuteAsync(new EditApiKeyCommand(scenario.GroupId, scenario.PhoneId, PlanningScenario.ApiKey));

        var settings = await GetAsync(host, scenario.GroupId, scenario.PhoneId);

        Assert.Equal(new GroupAgentSettingsDto(scenario.GroupId, scenario.PhoneId, "/ai", "google", PlanningScenario.ModelName, PlanningScenario.ApiKey), settings);
    }

    [Fact]
    public async Task MemberReadsTheSettingsWithoutTheKeyAndStrangerReadsNothing()
    {
        await using var host = await OrchestrationModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var scenario = await PlanningScenario.CreateAsync(host);

        var member = await GetAsync(host, scenario.GroupId, scenario.LaptopId);

        Assert.NotNull(member);
        Assert.Equal("/prompt", member.PromptMessagePrefix);
        Assert.Equal(PlanningScenario.ModelName, member.ClientModelName);
        Assert.Null(member.ApiKey);
        Assert.Null(await GetAsync(host, scenario.GroupId, scenario.StrangerId));
        Assert.Null(await GetAsync(host, Guid.NewGuid(), scenario.PhoneId));
    }

    [Fact]
    public async Task OnlyTheOwnerChangesTheSettings()
    {
        await using var host = await OrchestrationModuleHost.StartAsync(TestContext.Current.CancellationToken);
        var scenario = await PlanningScenario.CreateAsync(host);

        await AssertBrokenAsync<OnlyGroupOwnerCanChangeAgentSettingsRule>(() => host.ExecuteAsync(new EditApiKeyCommand(scenario.GroupId, scenario.LaptopId, "other-key")));
        await AssertBrokenAsync<OnlyGroupOwnerCanChangeAgentSettingsRule>(() => host.ExecuteAsync(new EditClientModelNameCommand(scenario.GroupId, scenario.LaptopId, "other-model")));
        await AssertBrokenAsync<OnlyGroupOwnerCanChangeAgentSettingsRule>(() => host.ExecuteAsync(new EditChatClientProviderNameCommand(scenario.GroupId, scenario.StrangerId, "openai")));
        await AssertBrokenAsync<OnlyGroupOwnerCanChangeAgentSettingsRule>(() => host.ExecuteAsync(new EditPromptMessagePrefixCommand(scenario.GroupId, scenario.LaptopId, "/x")));
        await AssertBrokenAsync<PromptMessagePrefixMustBeProvidedRule>(() => host.ExecuteAsync(new EditPromptMessagePrefixCommand(scenario.GroupId, scenario.PhoneId, "/ my")));

        var settings = await GetAsync(host, scenario.GroupId, scenario.PhoneId);
        Assert.Equal(new GroupAgentSettingsDto(scenario.GroupId, scenario.PhoneId, "/prompt", "google", PlanningScenario.ModelName, PlanningScenario.ApiKey), settings);
    }

    private static async Task<GroupAgentSettingsDto?> GetAsync(OrchestrationModuleHost host, Guid groupId, Guid requesterId)
    {
        return await host.QueryAsync<GetGroupAgentSettingsQuery, GroupAgentSettingsDto?>(new GetGroupAgentSettingsQuery(groupId, requesterId));
    }

    private static async Task AssertBrokenAsync<TRule>(Func<Task> action)
    {
        var exception = await Assert.ThrowsAsync<BuisnessRuleValidationException>(action);
        Assert.IsType<TRule>(exception.BrokenRule);
    }
}
