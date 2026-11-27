using Dara.Server.Modules.Groups.Application;
using Dara.Server.Modules.Groups.Application.Groups.CreateNewGroup;
using Dara.Server.Modules.Groups.Application.Groups.JoinToGroup;

using Dara.Server.Apps.API.Common.Seeding;

namespace Dara.Server.Apps.API.Modules.Groups;

public class GroupsSeeder : ISeeder
{
    private readonly IGroupsModule _groupsModule;
    private readonly ILogger<GroupsSeeder> _logger;

    public GroupsSeeder(IGroupsModule groupsModule, ILogger<GroupsSeeder> logger)
    {
        _groupsModule = groupsModule;
        _logger = logger;
    }

    public async Task SeedAsync(SeedData data, CancellationToken cancellationToken)
    {
        foreach (var group in data.Groups)
        {
            var groupId = await _groupsModule.ExecuteCommandAsync<CreateNewGroupCommand, Guid>(
                new CreateNewGroupCommand(data.ClientIds[group.Owner], group.Name, group.JoinCode));

            foreach (var member in group.Members)
                await _groupsModule.ExecuteCommandAsync(new JoinToGroupCommand(groupId, data.ClientIds[member], group.JoinCode));

            data.GroupIds.Add(group.Name, groupId);
        }

        _logger.LogInformation("Seeded {Count} groups", data.GroupIds.Count);
    }
}
