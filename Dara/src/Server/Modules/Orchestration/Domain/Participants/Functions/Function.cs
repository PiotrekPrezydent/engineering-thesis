using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Orchestration.Domain.Participants.Functions;

public class Function : Entity
{
    public FunctionId Id { get; private set; }
    
    public Participant Participant { get; private set; }
    public ParticipantId ParticipantId { get; private set; }
    
    public string Name { get => Participant.Name + "_" + field; private set; }
    public string Description { get; private set; }
    public string ReturnTypeName { get; private set; }
    
    public IReadOnlyList<FunctionParameter> Parameters { get; private set; }
    
    public Function(FunctionId id, string name, string description, string returnTypeName, IReadOnlyList<FunctionParameter> parameters)
    {
        Id = id;
        Name = name;
        Description = description;
        ReturnTypeName = returnTypeName;
        Parameters = parameters;
    }
}