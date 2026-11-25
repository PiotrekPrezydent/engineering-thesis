using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Orchestration.Domain.Participants.Functions.Descriptions;

namespace Dara.Server.Modules.Orchestration.Domain.Participants.Functions;

// a plugin function the participant can use now, a changed plugin comes with new function ids
public class Function : Entity
{
    public FunctionId Id { get; private set; }
    
    public Participant Participant { get; private set; }
    public ParticipantId ParticipantId { get; private set; }
    
    public PluginId PluginId { get; private set; }
    
    public string Name { get; private set; }
    public string? Description { get; private set; }
    
    public IReadOnlyList<PropertyDescription> Parameters { get; private set; }

    // null represent void function
    public TypeDescription? Returns { get; private set; }

    private Function()
    {
    }

    private Function(FunctionId id, ParticipantId participantId, PluginId pluginId, FunctionDescription description)
    {
        Id = id;
        ParticipantId = participantId;
        PluginId = pluginId;
        Name = description.Name;
        Description = description.Description;
        Parameters = description.Parameters;
        Returns = description.Returns;
    }

    internal static Function Create(FunctionId id, ParticipantId participantId, PluginId pluginId, FunctionDescription description)
    {
        return new Function(id, participantId, pluginId, description);
    }

    public FunctionDescription GetDescription()
    {
        return new FunctionDescription(Name, Description, Parameters, Returns);
    }

    internal FunctionSnapshot ToSnapshot()
    {
        return new FunctionSnapshot(Id.Value, PluginId.Value, GetDescription());
    }
}