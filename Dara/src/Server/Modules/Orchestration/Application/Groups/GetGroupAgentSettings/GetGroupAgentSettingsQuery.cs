using Dara.Server.BuildingBlocks.Application.Queries;

namespace Dara.Server.Modules.Orchestration.Application.Groups.GetGroupAgentSettings;

// participants see the settings, only the owner gets the api key; null for anyone outside the group
public record GetGroupAgentSettingsQuery(Guid GroupId, Guid RequesterId) : IQuery<GroupAgentSettingsDto?>;
