using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Orchestration.Domain.ParticipantGroups;
using Dara.Server.Modules.Orchestration.Domain.Participants.Functions;

namespace Dara.Server.Modules.Orchestration.Domain.Participants;

public class Participant : Entity, IAggregateRoot
{
    public ParticipantId Id { get; private set; }
    public string Name { get; private set; }
    
    public IReadOnlyList<Function> Functions  => _functions.AsReadOnly();
    private List<Function> _functions;

    private Participant()
    {
    }

    private Participant(ParticipantId id, string name)
    {
        Id = id;
        Name = name;
        _functions = new();
    }

    public static Participant CreateDefault(ParticipantId id)
    {
        return new Participant(id, "DEFAULT-NAME");
    }
    
    public void UpdateName(string name)
    {
        Name = name;
    }

    public void AddFunction(FunctionId id, string name, string description, string returnTypeName, IReadOnlyList<FunctionParameter> parameters)
    {
        _functions.Add(Function.Create(id, name, description, returnTypeName, parameters));
    }

    public void RemoveFunction(FunctionId id)
    {
        var function = _functions.Single(f => f.Id == id);
        _functions.Remove(function);
    }
}