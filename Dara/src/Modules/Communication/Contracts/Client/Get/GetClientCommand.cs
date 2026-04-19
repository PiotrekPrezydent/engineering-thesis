using Dara.BuildingBlocks.Contracts;

namespace Dara.Modules.Communication.Contracts.Client.Get;

public record GetClientCommand() : IApplicationCommand;

public record GetClientCommandResult() : IApplicationCommandResult;