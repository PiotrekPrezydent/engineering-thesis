namespace Dara.Shared.Contracts.Communication;

public record GroupDto(string GroupName, string GroupCode);

public record JoinGroupRequest(GroupDto GroupDto); //check if group exist, check if connection has registered client, check if client already have group

public record LeaveGroupRequest(); // check if client is in group

public record CreateGroupRequest(GroupDto GroupDto);

public record ChangeGroupNameRequest(string GivenAuthToken, string NewGroupName); // check if GivenAuthToken is owner of group

public record ChangeGroupCodeRequest(string GivenAuthToken, string NewGroupCode); // as above