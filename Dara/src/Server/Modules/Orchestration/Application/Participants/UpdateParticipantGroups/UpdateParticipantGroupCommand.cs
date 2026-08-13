using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Orchestration.Application.Participants.UpdateParticipantGroups;

public record UpdateParticipantGroupCommand(Guid ParticipantId, Guid GroupId, UpdateProjectionListOption Option) : ICommand;