using Dara.BuildingBlocks.Contracts;

namespace Dara.Modules.Communication.Contracts.Group;

public record DestroyGroupCommand() : IApplicationCommand;

public record RemoveGroupCommandResult() : IApplicationCommandResult;