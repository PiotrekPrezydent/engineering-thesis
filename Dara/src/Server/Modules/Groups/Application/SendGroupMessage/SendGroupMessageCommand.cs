using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Groups.Application.SendGroupMessage;

public record SendGroupMessageCommand(Guid GroupId, Guid AuthorId, string Content) : ICommand;