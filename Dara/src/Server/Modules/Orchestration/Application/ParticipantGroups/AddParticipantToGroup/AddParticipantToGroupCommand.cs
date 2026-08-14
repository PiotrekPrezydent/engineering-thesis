using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Orchestration.Application.ParticipantGroups.AddParticipantToGroup;

public record AddParticipantToGroupCommand(Guid GroupId, Guid ParticipantId) : ICommand;