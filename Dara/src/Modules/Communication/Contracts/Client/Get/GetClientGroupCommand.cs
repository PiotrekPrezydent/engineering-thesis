using Dara.BuildingBlocks.Contracts;

namespace Dara.Modules.Communication.Contracts.Client.Get;

public record GetClientGroupCommand() : IApplicationCommand;

public record GetClientGroupCommandResult() : IApplicationCommandResult;