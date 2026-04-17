using Dara.BuildingBlocks.Contracts;

namespace Dara.Modules.RpcGateway.Contracts;

public record ChangeClientNameCommand() : IApplicationCommand;

public record ChangeClientNameCommandResult() : IApplicationCommandResult;