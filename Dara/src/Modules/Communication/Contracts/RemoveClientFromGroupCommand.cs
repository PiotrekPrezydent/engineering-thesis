using Dara.BuildingBlocks.Contracts;

namespace Dara.Modules.Communication.Contracts;

public record RemoveClientFromGroupCommand() : IApplicationCommand;

public record RemoveClientFromGroupCommandResult() : IApplicationCommandResult;