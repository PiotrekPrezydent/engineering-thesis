using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Orchestration.Domain.Participants;

namespace Dara.Server.Modules.Orchestration.Application.Participants.MarkParticipantRevoked;

public class MarkParticipantRevokedCommandHandler : ICommandHandler<MarkParticipantRevokedCommand>
{
    private readonly IParticipantRepository _repository;

    public MarkParticipantRevokedCommandHandler(IParticipantRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(MarkParticipantRevokedCommand command)
    {
        var participant = await _repository.GetByIdAsync(new(command.ParticipantId));
        participant.MarkRevoked();
    }
}
