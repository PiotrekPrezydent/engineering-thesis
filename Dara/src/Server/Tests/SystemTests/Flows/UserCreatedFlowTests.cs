using Dara.Server.Modules.Groups.Domain.Members;
using Dara.Server.Modules.Identity.Application.GetUser;
using Dara.Server.Modules.Orchestration.Domain.Participants;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Dara.Server.Modules.Profiles.Application.ChangeProfileName;
using Dara.Server.Modules.Profiles.Application.GetProfile;
using Dara.Server.SystemTests.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.SystemTests.Flows;

public class UserCreatedFlowTests
{
    [Fact]
    public async Task NewUserIsProjectedToProfilesAndGroups()
    {
        await using var host = await ModulesHost.StartAsync(TestContext.Current.CancellationToken);

        var userId = await host.CreateUserAsync("flow-user");

        await ModulesHost.WaitUntilAsync(() => ProfileExistsAsync(host, userId), "profile of the new user");
        await ModulesHost.WaitUntilAsync(() => MemberExistsAsync(host, userId, "DEFAULT-NAME"), "groups member of the new user");

        var profile = await host.Profiles.ExecuteQueryAsync<GetProfileQuery, ProfileDto>(new GetProfileQuery(userId));
        Assert.Equal("DEFAULT-NAME", profile.Name);
    }

    [Fact]
    public async Task ChangedProfileNameReachesGroups()
    {
        await using var host = await ModulesHost.StartAsync(TestContext.Current.CancellationToken);

        var userId = await host.CreateUserAsync("renamed-user");
        await ModulesHost.WaitUntilAsync(() => ProfileExistsAsync(host, userId), "profile of the new user");

        await host.Profiles.ExecuteCommandAsync(new ChangeProfileNameCommand(userId, "Bob-Phone"));

        await ModulesHost.WaitUntilAsync(() => MemberExistsAsync(host, userId, "Bob-Phone"), "groups member renamed");
    }

    // plugins is subscribed before orchestration, so this also shows that the plugins ef model builds
    [Fact]
    public async Task NewUserIsProjectedToPluginsAndOrchestration()
    {
        await using var host = await ModulesHost.StartAsync(TestContext.Current.CancellationToken);

        var userId = await host.CreateUserAsync("participant-user");

        await ModulesHost.WaitUntilAsync(() => ModulesHost.ReadAsync(host.PluginsRoot,
            model => model.Query<PluginOwner>().AnyAsync(e => e.Id.Match(userId))), "plugin owner of the new user");
        await ModulesHost.WaitUntilAsync(() => ParticipantExistsAsync(host, userId, "DEFAULT-NAME"), "participant of the new user");

        await host.Profiles.ExecuteCommandAsync(new ChangeProfileNameCommand(userId, "Bob-Laptop"));

        await ModulesHost.WaitUntilAsync(() => ParticipantExistsAsync(host, userId, "Bob-Laptop"), "participant renamed");
    }

    [Fact]
    public async Task HostsDoNotShareEventsOrData()
    {
        await using var first = await ModulesHost.StartAsync(TestContext.Current.CancellationToken);
        await using var second = await ModulesHost.StartAsync(TestContext.Current.CancellationToken);

        var userId = await first.CreateUserAsync("first-host-user");
        await ModulesHost.WaitUntilAsync(() => ProfileExistsAsync(first, userId), "profile in the first host");

        var secondUsers = await second.Identity.ExecuteQueryAsync<GetAllUsersQuery, List<(Guid userId, string login)>>(new GetAllUsersQuery());
        Assert.DoesNotContain(secondUsers, user => user.userId == userId);
        Assert.False(await ProfileExistsAsync(second, userId));
    }

    private static async Task<bool> ProfileExistsAsync(ModulesHost host, Guid userId)
    {
        var profiles = await host.Profiles.ExecuteQueryAsync<GetProfilesQuery, List<ProfileDto>>(new GetProfilesQuery([userId]));
        return profiles.Count == 1;
    }

    private static Task<bool> MemberExistsAsync(ModulesHost host, Guid userId, string name)
        => ModulesHost.ReadAsync(host.GroupsRoot, model => model.Query<Member>().AnyAsync(e => e.Id.Match(userId) && e.Name == name));

    private static Task<bool> ParticipantExistsAsync(ModulesHost host, Guid userId, string name)
        => ModulesHost.ReadAsync(host.OrchestrationRoot, model => model.Query<Participant>().AnyAsync(e => e.Id.Match(userId) && e.Name == name));
}
