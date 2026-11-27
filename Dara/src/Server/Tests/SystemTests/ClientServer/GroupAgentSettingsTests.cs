using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.SystemTests.ClientServer;

// agent settings of a group over the hub: participants read them, only the owner edits them and sees the api key
public class GroupAgentSettingsTests
{
    private const string ApiKey = "settings-test-key-not-real";

    [Fact]
    public async Task OwnerEditsTheSettingsAndOnlyTheOwnerReadsTheKey()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var owner = await host.ConnectAsync("settings-owner", cancellationToken);
        var member = await host.ConnectAsync("settings-member", cancellationToken);
        var outsider = await host.ConnectAsync("settings-outsider", cancellationToken);
        var group = await host.CreateGroupAsync(owner, "settings", member);

        await owner.Hub.EditGroupPromptMessagePrefix(group.GroupId, "/dara");
        await owner.Hub.EditGroupChatClientProviderName(group.GroupId, "google");
        await owner.Hub.EditGroupClientModelName(group.GroupId, "gemini-test");
        await owner.Hub.EditGroupApiKey(group.GroupId, ApiKey);

        var atOwner = await owner.Hub.GetGroupAgentSettings(group.GroupId);
        var atMember = await member.Hub.GetGroupAgentSettings(group.GroupId);
        Assert.Equal(("/dara", "google", "gemini-test", ApiKey),
            (atOwner!.PromptMessagePrefix, atOwner.ChatClientProviderName, atOwner.ClientModelName, atOwner.ApiKey));
        Assert.Equal(owner.ClientId, atMember!.OwnerId);
        Assert.Equal(("/dara", "google", "gemini-test"), (atMember.PromptMessagePrefix, atMember.ChatClientProviderName, atMember.ClientModelName));
        Assert.Null(atMember.ApiKey);
        Assert.Null(await outsider.Hub.GetGroupAgentSettings(group.GroupId));
    }

    [Fact]
    public async Task MemberCannotEditTheSettings()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var owner = await host.ConnectAsync("guarded-owner", cancellationToken);
        var member = await host.ConnectAsync("guarded-member", cancellationToken);
        var group = await host.CreateGroupAsync(owner, "guarded", member);

        var keyError = await Assert.ThrowsAsync<HubException>(() => member.Hub.EditGroupApiKey(group.GroupId, "stolen"));
        var prefixError = await Assert.ThrowsAsync<HubException>(() => owner.Hub.EditGroupPromptMessagePrefix(group.GroupId, "with space"));

        Assert.Contains("Only the group owner can change the agent settings of the group", keyError.Message);
        Assert.Contains("Prompt message prefix must be provided", prefixError.Message);
        var settings = await owner.Hub.GetGroupAgentSettings(group.GroupId);
        Assert.Equal(("/prompt", ""), (settings!.PromptMessagePrefix, settings.ApiKey));
    }
}
