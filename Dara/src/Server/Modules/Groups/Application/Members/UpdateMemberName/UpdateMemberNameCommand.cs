using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Groups.Application.Members.UpdateMemberName;

public record UpdateMemberNameCommand(Guid MemberId, string NewName) : ICommand;