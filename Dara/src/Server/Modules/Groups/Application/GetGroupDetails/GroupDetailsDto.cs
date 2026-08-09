namespace Dara.Server.Modules.Groups.Application.GetGroupDetails;

public record GroupDetailsDto(Guid GroupId, Guid OwnerId, string GroupName, string JoinCode, List<Guid> Members);