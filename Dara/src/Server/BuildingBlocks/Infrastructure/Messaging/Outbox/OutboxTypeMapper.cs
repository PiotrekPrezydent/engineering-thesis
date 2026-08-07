namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox;

public class OutboxTypeMapper : IOutboxTypeMapper
{
    private readonly IDictionary<string, Type> _namesToTypeOutboxMap;
    private readonly IDictionary<Type, string> _typesToNamesOutboxMap;
    
    public OutboxTypeMapper(IDictionary<string, Type> namesToTypeOutboxMap)
    {
        _namesToTypeOutboxMap = namesToTypeOutboxMap;
        _typesToNamesOutboxMap = new Dictionary<Type, string>();
        foreach (var kvp in namesToTypeOutboxMap)
            _typesToNamesOutboxMap.Add(kvp.Value, kvp.Key);
    }
    

    public string GetName(Type type)
    {
        return _typesToNamesOutboxMap[type];
    }
    
    public Type GetType(string name)
    {
        return _namesToTypeOutboxMap[name];
    }

    public bool CanHandleType(Type type)
    {
        return _typesToNamesOutboxMap.ContainsKey(type);
    }
}