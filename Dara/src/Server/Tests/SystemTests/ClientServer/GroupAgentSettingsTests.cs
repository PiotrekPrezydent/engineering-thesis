using System.Net;

namespace Dara.Server.SystemTests.ClientServer;

// agent settings of a group over http: only the panel of the account of the owner client reads and edits them
public class GroupAgentSettingsTests
{
    private const string ApiKey = "settings-test-key-not-real";

    [Fact]
    public async Task OwnerEditsTheSettingsAndOnlyThePanelOfTheOwnerReadsThem()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var owner = await host.ConnectAsync("settings-owner", cancellationToken);
        var member = await host.ConnectAsync("settings-member", cancellationToken);
        var group = await host.CreateGroupAsync(owner, "settings", member);
        var ownerPanel = await host.PanelOfAsync(owner);
        var memberPanel = await host.PanelOfAsync(member);

        await AppHubTestHost.EnsureAsync(await ownerPanel.EditPromptPrefixAsync(group.GroupId, "/dara"), "prefix");
        await ownerPanel.SetModelAsync(group.GroupId, "google", "gemini-test", ApiKey);

        var settings = await ownerPanel.ReadAgentSettingsAsync(group.GroupId);
        Assert.Equal(("/dara", "google", "gemini-test", ApiKey),
            (settings.PromptMessagePrefix, settings.ChatClientProviderName, settings.ClientModelName, settings.ApiKey));
        Assert.Equal(owner.ClientId, settings.OwnerId);
        Assert.Equal(HttpStatusCode.NotFound, (await memberPanel.GetAgentSettingsAsync(group.GroupId)).StatusCode);
    }

    [Fact]
    public async Task OtherAccountCannotEditAndAWrongPrefixIsARuleOfTheCaller()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var owner = await host.ConnectAsync("guarded-owner", cancellationToken);
        var member = await host.ConnectAsync("guarded-member", cancellationToken);
        var group = await host.CreateGroupAsync(owner, "guarded", member);
        var ownerPanel = await host.PanelOfAsync(owner);
        var memberPanel = await host.PanelOfAsync(member);

        var stolen = await PanelTestClient.ProblemAsync(await memberPanel.EditApiKeyAsync(group.GroupId, "stolen"));
        var prefix = await PanelTestClient.ProblemAsync(await ownerPanel.EditPromptPrefixAsync(group.GroupId, "with space"));

        Assert.Equal((404, "not_found"), (stolen.Status, stolen.Code));
        Assert.Equal((400, "rule_broken", "PromptMessagePrefixMustBeProvidedRule"), (prefix.Status, prefix.Code, prefix.Rule));
        Assert.Contains("Prompt message prefix must be provided", prefix.Detail);
        var settings = await ownerPanel.ReadAgentSettingsAsync(group.GroupId);
        Assert.Equal(("/prompt", ""), (settings.PromptMessagePrefix, settings.ApiKey));
    }
}
