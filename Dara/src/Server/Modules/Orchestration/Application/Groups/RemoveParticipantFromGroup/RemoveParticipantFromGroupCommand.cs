using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Orchestration.Application.Groups.RemoveParticipantFromGroup;

public record RemoveParticipantFromGroupCommand(Guid GroupId, Guid ParticipantId) : ICommand;