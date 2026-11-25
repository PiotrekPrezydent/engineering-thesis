using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Orchestration.Domain.Groups;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Orchestration.Application.Groups.GetGroupAgentSettings;

public class GetGroupAgentSettingsQueryHandler : IQueryHandler<GetGroupAgentSettingsQuery, GroupAgentSettingsDto?>
{
    private readonly IReadModel _readModel;

    public GetGroupAgentSettingsQueryHandler(IReadModel readModel)
    {
        _readModel = readModel;
    }

    public async Task<GroupAgentSettingsDto?> HandleAsync(GetGroupAgentSettingsQuery query)
    {
        var group = await _readModel.Query<Group>().FirstOrDefaultAsync(g => g.Id.Match(query.GroupId));
        if (group is null || !group.Participants.Any(p => p.Match(query.RequesterId)))
            return null;

        return new GroupAgentSettingsDto(
            group.Id.Value,
            group.OwnerId.Value,
            group.AgentSettings.PromptMessagePrefix,
            group.AgentSettings.ChatClientProviderName,
            group.AgentSettings.ClientModelName,
            group.OwnerId.Match(query.RequesterId) ? group.ApiKey : null);
    }
}
