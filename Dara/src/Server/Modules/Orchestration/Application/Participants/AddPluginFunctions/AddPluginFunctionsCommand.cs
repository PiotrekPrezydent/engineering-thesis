using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Orchestration.Application.Participants.AddPluginFunctions;

public record AddPluginFunctionsCommand(Guid ParticipantId, IReadOnlyList<FunctionData> Functions) : ICommand;