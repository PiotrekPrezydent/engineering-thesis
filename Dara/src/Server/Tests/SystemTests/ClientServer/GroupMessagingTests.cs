using Dara.Shared.Contracts.Notifications;
using Dara.Shared.Contracts.StateSnapshots;

namespace Dara.Server.SystemTests.ClientServer;

// messages over http, notifications of groups and names over the hub; signalr keeps the order of messages to one connection,
// so a later notification of a client proves that an earlier one did not come
public class GroupMessagingTests
{
    [Fact]
    public async Task GroupMessageReachesEveryMemberAndNobodyElse()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var owner = await host.ConnectAsync("owner-phone", cancellationToken);
        var member = await host.ConnectAsync("member-laptop", cancellationToken);
        var outsider = await host.ConnectAsync("outsider-tablet", cancellationToken);

        var group = await host.CreateGroupAsync(owner, "family", member);
        await owner.Events.WaitForAsync<GroupMemberJoinedGroupNotification>(n => n.GroupId == group.GroupId && n.GroupMember.MemberId == member.ClientId,
            "owner told about the new member");

        await member.Api.SendGroupMessageAsync(group.GroupId, "dinner at seven", cancellationToken);

        var atOwner = await owner.Events.WaitForAsync<GroupMessageReceivedNotification>(m => m.Message == "dinner at seven", "message at the owner");
        var atMember = await member.Events.WaitForAsync<GroupMessageReceivedNotification>(m => m.Message == "dinner at seven", "message at its author");
        Assert.Equal(group.GroupId, atOwner.GroupId);
        Assert.Equal(member.ClientId, atOwner.MessageAuthorId);
        Assert.NotEqual(Guid.Empty, atOwner.MessageId);
        Assert.Equal(atOwner.MessageId, atMember.MessageId);

        await host.RenameClientAsync(outsider, "Outsider");
        await outsider.Events.WaitForAsync<ClientRenamedNotification>(n => n.Name == "Outsider", "outsider renamed");
        Assert.Empty(outsider.Events.Received<GroupMessageReceivedNotification>());
    }

    [Fact]
    public async Task ConnectionDoesNotTellTheClientItLeftAGroup()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var phone = await host.ConnectAsync("reconnecting-phone", cancellationToken);

        await phone.ReconnectAsync(cancellationToken);
        await host.RenameClientAsync(phone, "Bob-Phone");

        var renamed = await phone.Events.WaitForAsync<ClientRenamedNotification>(n => n.Name == "Bob-Phone", "name changed");
        Assert.Equal(phone.ClientId, renamed.ClientId);
        Assert.Empty(phone.Events.Received<GroupLeft>());
    }

    [Fact]
    public async Task WrongJoinCodeIsRefusedForThePanel()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var owner = await host.ConnectAsync("code-owner", cancellationToken);
        var guest = await host.ConnectAsync("code-guest", cancellationToken);
        var group = await host.CreateGroupAsync(owner, "locked");

        var guestPanel = await host.PanelOfAsync(guest);
        var error = await PanelTestClient.ProblemAsync(await guestPanel.JoinGroupAsync(guest.ClientId, group.GroupId, "wrong-code"));

        Assert.Equal((403, "forbidden", "ProvidedGroupJoiningCodeMustBeValidRule"), (error.Status, error.Code, error.Rule));
        Assert.Contains("Group code must be valid", error.Detail);
        Assert.Empty(guest.Events.Received<GroupSnapshot>());
    }
}
