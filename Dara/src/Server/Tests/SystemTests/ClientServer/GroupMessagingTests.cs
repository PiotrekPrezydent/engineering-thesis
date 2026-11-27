using Dara.Shared.Contracts.Notifications;
using Dara.Shared.Contracts.StateSnapshots;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.SystemTests.ClientServer;

// groups, messages and names over the hub; signalr keeps the order of messages to one connection,
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

        await member.Hub.SendGroupMessage(group.GroupId, "dinner at seven");

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
    public async Task WrongJoinCodeIsAnErrorForTheCaller()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var owner = await host.ConnectAsync("code-owner", cancellationToken);
        var guest = await host.ConnectAsync("code-guest", cancellationToken);
        var group = await host.CreateGroupAsync(owner, "locked");

        var error = await Assert.ThrowsAsync<HubException>(() => guest.Hub.JoinGroup(group.GroupId, "wrong-code"));

        Assert.Contains("Group code must be valid", error.Message);
        Assert.Empty(guest.Events.Received<GroupSnapshot>());
    }
}
