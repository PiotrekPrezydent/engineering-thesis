using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Groups.Application.JoinToGroup;

public record JoinToGroupCommand(Guid GroupId, Guid MemberId, string JoinCode) : ICommand;