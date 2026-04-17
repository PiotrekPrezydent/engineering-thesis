using Dara.BuildingBlocks.Contracts;

namespace Dara.Modules.RpcGateway.Contracts;

public record RemoveConnectionCommand() : IApplicationCommand;

public record RemoveConnectionCommandResult() : IApplicationCommandResult;


