using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Orchestration.Domain.Participants.Functions;

namespace Dara.Server.Modules.Orchestration.Domain.Participants;

// a client with the plugin functions it can use now, projected from identity and plugins
public class Participant : Entity, IAggregateRoot
{
    public ParticipantId Id { get; private set; }
    public string Name { get; private set; }

    public bool IsRevoked { get; private set; }

    // the context of the client, replaced as a whole
    public IReadOnlyDictionary<string, string> Context { get; private set; } = new Dictionary<string, string>();

    public IReadOnlyList<Function> Functions  => _functions.AsReadOnly();
    private List<Function> _functions;

    // kept after a plugin is gone, so an older state that comes late changes nothing
    public IReadOnlyList<PluginVersion> PluginVersions => _pluginVersions.AsReadOnly();
    private List<PluginVersion> _pluginVersions;

    private Participant()
    {
    }

    private Participant(ParticipantId id, string name)
    {
        Id = id;
        Name = name;
        _functions = new();
        _pluginVersions = new();
    }

    public static Participant Create(ParticipantId id, string name)
    {
        return new Participant(id, name);
    }

    // a placeholder for plugin functions that came before the client; the event of the client fills it in
    public static Participant CreateDefault(ParticipantId id)
    {
        return new Participant(id, "DEFAULT-NAME");
    }

    public void MarkRevoked()
    {
        IsRevoked = true;
    }

    public void ChangeContext(IReadOnlyDictionary<string, string> context)
    {
        Context = new Dictionary<string, string>(context);
    }


    public void UpdateName(string name)
    {
        Name = name;
    }

    // only a newer state of the plugin replaces its functions, an empty list removes them
    public bool ReplacePluginFunctions(PluginId pluginId, int version, IReadOnlyList<AvailableFunction> functions)
    {
        var applied = _pluginVersions.FirstOrDefault(v => v.PluginId == pluginId);
        if (applied is not null && applied.Version >= version)
            return false;

        var availableIds = functions.Select(f => f.Id).ToHashSet();
        _functions.RemoveAll(f => f.PluginId == pluginId && !availableIds.Contains(f.Id));

        // kept functions stay the same instances, ef cannot track a removed and an added row with one key
        var keptIds = _functions.Select(f => f.Id).ToHashSet();
        _functions.AddRange(functions
            .Where(f => !keptIds.Contains(f.Id))
            .Select(f => Function.Create(f.Id, Id, pluginId, f.Description)));

        _pluginVersions = _pluginVersions
            .Where(v => v.PluginId != pluginId)
            .Append(new PluginVersion(pluginId, version))
            .ToList();

        return true;
    }

    public ParticipantSnapshot ToSnapshot()
    {
        return new ParticipantSnapshot(
            Id.Value,
            Name,
            Functions.Select(e => e.ToSnapshot()).ToList());
    }
}