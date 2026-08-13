using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Orchestration.Application.Participants.UpdateParticipantName;

public record UpdateParticipantNameCommand(Guid ParticipantId, string NewName) : ICommand;