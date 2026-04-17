using Dara.BuildingBlocks.Contracts;

namespace Dara.Modules.RpcGateway.Contracts;

public record RemoveConnectionClientCommand() : IApplicationCommand;

public record RemoveConnectionClientCommandResult() : IApplicationCommandResult;