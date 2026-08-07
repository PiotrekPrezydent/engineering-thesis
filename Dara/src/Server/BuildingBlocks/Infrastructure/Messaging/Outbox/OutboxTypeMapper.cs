namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox;

public class OutboxTypeMapper : IOutboxTypeMapper
{
    private readonly IDictionary<string, Type> _outboxTypesMap;
    private readonly IDictionary<Type, string> _outboxNamesMap;
    
    public OutboxTypeMapper(IDictionary<string, Type> outboxTypesMap)
    {
        _outboxTypesMap = outboxTypesMap;
        _outboxNamesMap = new Dictionary<Type, string>();
        foreach (var kvp in outboxTypesMap)
            _outboxNamesMap.Add(kvp.Value, kvp.Key);
    }

    public string GetName(Type type)
    {
        return _outboxNamesMap[type];
    }
    
    public Type GetType(string name)
    {
        return _outboxTypesMap[name];
    }
}