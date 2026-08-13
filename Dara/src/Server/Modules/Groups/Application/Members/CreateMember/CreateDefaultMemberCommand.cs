using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Groups.Application.Members.CreateMember;

public record CreateDefaultMemberCommand(Guid MemberId) : ICommand;