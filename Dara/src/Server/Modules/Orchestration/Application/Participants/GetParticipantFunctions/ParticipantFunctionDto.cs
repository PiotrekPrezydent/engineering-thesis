namespace Dara.Server.Modules.Orchestration.Application.Participants.GetParticipantFunctions;

public record ParticipantFunctionDto(Guid FunctionId, Guid PluginId, string Name, string? Description);
