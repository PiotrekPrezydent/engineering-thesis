using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Orchestration.Application.ParticipantGroups.CreateParticipantGroup;

public record CreateParticipantGroupCommand(Guid GroupId, Guid CreatorId) : ICommand;