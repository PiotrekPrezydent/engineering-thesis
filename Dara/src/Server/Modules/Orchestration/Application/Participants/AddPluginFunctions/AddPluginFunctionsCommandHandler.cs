using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Orchestration.Domain.Participants;
using Dara.Server.Modules.Orchestration.Domain.Participants.Functions;

namespace Dara.Server.Modules.Orchestration.Application.Participants.AddPluginFunctions;

public class AddPluginFunctionsCommandHandler : ICommandHandler<AddPluginFunctionsCommand>
{
    private readonly IParticipantRepository _repository;

    public AddPluginFunctionsCommandHandler(IParticipantRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(AddPluginFunctionsCommand command)
    {
        var participant = await _repository.GetByIdAsync(new ParticipantId(command.ParticipantId));
        foreach (var functionData in command.Functions)
        {
            participant.AddFunction(
                new(functionData.Id), 
                functionData.Name,
                functionData.Description,
                functionData.ReturnTypeName,
                functionData.Parameters
                    .Select(p=>new FunctionParameter(p.Name,p.Description,p.TypeName))
                    .ToList()
                );
        }
    }
}