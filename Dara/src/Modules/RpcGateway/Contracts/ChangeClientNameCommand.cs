using Dara.BuildingBlocks.Contracts;

namespace Dara.Modules.RpcGateway.Contracts;

public record ChangeClientNameCommand(string Name) : IApplicationCommand;

public record ChangeClientNameCommandResult() : IApplicationCommandResult;