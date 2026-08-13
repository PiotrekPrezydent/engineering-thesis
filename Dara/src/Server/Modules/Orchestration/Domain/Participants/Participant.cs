using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Orchestration.Domain.Participants.Functions;

namespace Dara.Server.Modules.Orchestration.Domain.Participants;

public class Participant : Entity, IAggregateRoot
{
    public ParticipantId Id { get; private set; }
    public string Name { get; private set; }
    
    public IReadOnlyList<ParticipantGroupId> Groups => _groups.AsReadOnly();
    private List<ParticipantGroupId> _groups;
    
    public IReadOnlyList<Function> Functions  => _functions.AsReadOnly();
    private List<Function> _functions;

    private Participant(ParticipantId id, string name)
    {
        Id = id;
        Name = name;
        _groups = new();
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

    public void AddGroup(ParticipantGroupId group)
    {
        _groups.Add(group);
    }
    
    public void RemoveGroup(ParticipantGroupId group)
    {
        _groups.Remove(group);
    }
    
    public bool IsMemberOfGroup(ParticipantGroupId group) => _groups.Contains(group);

    public void AddFunction(Function function)
    {
        _functions.Add(function);
    }

    public void RemoveFunction(Function function)
    {
        _functions.Remove(function);
    }
}