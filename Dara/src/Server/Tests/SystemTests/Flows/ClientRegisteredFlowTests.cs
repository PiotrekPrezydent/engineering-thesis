using Dara.Server.Modules.Groups.Domain.Members;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.ChangeClientContext;
using Dara.Server.Modules.Identity.Application.Clients;
using Dara.Server.Modules.Identity.Application.GetClients;
using Dara.Server.Modules.Identity.Application.GetUser;
using Dara.Server.Modules.Identity.Application.RevokeClient;
using Dara.Server.Modules.Identity.Domain.ClientSessions;
using Dara.Server.Modules.Orchestration.Domain.Participants;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Dara.Server.SystemTests.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.SystemTests.Flows;

// a client registers itself in identity with a code of the panel; groups, orchestration and plugins keep their
// projections of it from the UserClient* integration events of identity
public class ClientRegisteredFlowTests
{
    [Fact]
    public async Task ClientsOfOneAccountAreProjectedToAllModules()
    {
        await using var host = await ModulesHost.StartAsync(TestContext.Current.CancellationToken);

        var userId = await host.CreateUserAsync("flow-user");
        var pc = await host.RegisterClientAsync(userId);
        var phone = await host.RegisterClientAsync(userId, "Bob-Phone", "Phone");

        foreach (var (clientId, name) in new[] { (pc.ClientId, "PC 1"), (phone.ClientId, "Bob-Phone") })
        {
            Assert.True(await ClientSessionExistsAsync(host, userId, clientId), $"identity session of {name}");
            await ModulesHost.WaitUntilAsync(() => ReadMemberAsync(host, clientId, m => m.UserId == userId && m.Name == name && !m.IsRevoked), $"groups member {name}");
            await ModulesHost.WaitUntilAsync(() => ReadParticipantAsync(host, clientId, p => p.Name == name && !p.IsRevoked), $"participant {name}");
            await ModulesHost.WaitUntilAsync(() => ModulesHost.ReadAsync(host.PluginsRoot,
                model => model.Query<PluginOwner>().AnyAsync(e => e.Id.Match(clientId))), $"plugin owner {name}");
        }

        var clients = await host.Identity.ExecuteQueryAsync<GetAccountClientsQuery, List<UserClientDto>>(new GetAccountClientsQuery(userId));
        Assert.Equal([pc.ClientId, phone.ClientId], clients.Select(c => c.ClientId));

        // an existing client signs in again with a login code of the panel
        var tokens = await host.LoginClientAsync(userId, phone.ClientId);
        Assert.True(tokens.Succeeded, tokens.Error?.ToString());
        Assert.Equal((userId, phone.ClientId), (tokens.UserId, tokens.ClientId));
    }

    [Fact]
    public async Task ChangesOfAClientReachTheProjections()
    {
        await using var host = await ModulesHost.StartAsync(TestContext.Current.CancellationToken);

        var laptop = await host.CreateClientAsync("changed-user", "Bob-Laptop");
        await ModulesHost.WaitUntilAsync(() => ReadParticipantAsync(host, laptop.ClientId, _ => true), "participant of the laptop");
        await ModulesHost.WaitUntilAsync(() => ReadMemberAsync(host, laptop.ClientId, _ => true), "member of the laptop");

        await host.RenameClientAsync(laptop.UserId, laptop.ClientId, "Bob-Notebook");
        await host.ExecuteAsync(new ChangeClientContextCommand(laptop.UserId, laptop.ClientId,
            new Dictionary<string, string> { ["Folder zdjęć"] = @"C:\Users\bob\Pictures" }));

        await ModulesHost.WaitUntilAsync(() => ReadMemberAsync(host, laptop.ClientId, m => m.Name == "Bob-Notebook"), "groups member renamed");
        await ModulesHost.WaitUntilAsync(() => ReadParticipantAsync(host, laptop.ClientId,
            p => p.Name == "Bob-Notebook"
                 && p.Context.TryGetValue("Folder zdjęć", out var folder) && folder == @"C:\Users\bob\Pictures"), "participant with name and context");
    }

    [Fact]
    public async Task ARevokedClientIsMarkedEverywhereAndGetsNoLoginCode()
    {
        await using var host = await ModulesHost.StartAsync(TestContext.Current.CancellationToken);

        var userId = await host.CreateUserAsync("revoked-user");
        var pc = await host.RegisterClientAsync(userId);
        var phone = await host.RegisterClientAsync(userId, "Lost-Phone", "Phone");
        Assert.True((await host.LoginClientAsync(userId, phone.ClientId)).Succeeded);

        await host.ExecuteAsync(new RevokeClientCommand(userId, phone.ClientId));

        await ModulesHost.WaitUntilAsync(() => ReadMemberAsync(host, phone.ClientId, m => m.IsRevoked), "groups member revoked");
        await ModulesHost.WaitUntilAsync(() => ReadParticipantAsync(host, phone.ClientId, p => p.IsRevoked), "participant revoked");
        Assert.True(await ModulesHost.ReadAsync(host.IdentityRoot,
            model => model.Query<ClientSession>().AnyAsync(e => e.ClientId.Match(phone.ClientId) && e.RevokedAt != null)), "identity session revoked");

        var code = await host.IssueLoginCodeAsync(userId, phone.ClientId);
        Assert.Equal(AuthError.ClientRevoked, code.Error);

        // the other client of the account is not touched
        await ModulesHost.WaitUntilAsync(() => ReadParticipantAsync(host, pc.ClientId, p => !p.IsRevoked), "other participant");
        Assert.True((await host.IssueLoginCodeAsync(userId, pc.ClientId)).Succeeded);
    }

    [Fact]
    public async Task HostsDoNotShareEventsOrData()
    {
        await using var first = await ModulesHost.StartAsync(TestContext.Current.CancellationToken);
        await using var second = await ModulesHost.StartAsync(TestContext.Current.CancellationToken);

        var client = await first.CreateClientAsync("first-host-user");
        await ModulesHost.WaitUntilAsync(() => ReadParticipantAsync(first, client.ClientId, _ => true), "participant in the first host");

        var secondUsers = await second.Identity.ExecuteQueryAsync<GetAllUsersQuery, List<(Guid userId, string login)>>(new GetAllUsersQuery());
        Assert.DoesNotContain(secondUsers, user => user.userId == client.UserId);
        var secondClients = await second.Identity.ExecuteQueryAsync<GetClientsQuery, List<ClientDto>>(new GetClientsQuery([client.ClientId]));
        Assert.Empty(secondClients);
        Assert.False(await ReadParticipantAsync(second, client.ClientId, _ => true));
    }

    private static Task<bool> ClientSessionExistsAsync(ModulesHost host, Guid userId, Guid clientId)
        => ModulesHost.ReadAsync(host.IdentityRoot, model => model.Query<ClientSession>().AnyAsync(e => e.ClientId.Match(clientId) && e.UserId.Match(userId)));

    private static async Task<bool> ReadMemberAsync(ModulesHost host, Guid clientId, Func<Member, bool> condition)
    {
        var member = await ModulesHost.ReadAsync(host.GroupsRoot, model => model.Query<Member>().FirstOrDefaultAsync(e => e.Id.Match(clientId)));
        return member is not null && condition(member);
    }

    private static async Task<bool> ReadParticipantAsync(ModulesHost host, Guid clientId, Func<Participant, bool> condition)
    {
        var participant = await ModulesHost.ReadAsync(host.OrchestrationRoot, model => model.Query<Participant>().FirstOrDefaultAsync(e => e.Id.Match(clientId)));
        return participant is not null && condition(participant);
    }
}
