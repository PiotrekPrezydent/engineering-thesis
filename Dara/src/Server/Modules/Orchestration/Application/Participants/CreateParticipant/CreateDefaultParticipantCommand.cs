using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Orchestration.Application.Participants.CreateParticipant;

public record CreateDefaultParticipantCommand(Guid ParticipantId) : ICommand;