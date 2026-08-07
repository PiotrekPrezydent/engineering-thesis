namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Mapping;

public class InboxTypeMapper : IInboxTypeMapper
{
    private readonly IDictionary<string, Type> _namesToTypesInboxMap;
    private readonly IDictionary<Type, string> _typesToNamesInboxMap;
    
    public InboxTypeMapper(IDictionary<string, Type> namesToTypesInboxMap)
    {
        _namesToTypesInboxMap = namesToTypesInboxMap;
        _typesToNamesInboxMap = new Dictionary<Type, string>();
        foreach (var kvp in namesToTypesInboxMap)
            _typesToNamesInboxMap.Add(kvp.Value, kvp.Key);
    }
    

    public string GetName(Type type)
    {
        return _typesToNamesInboxMap[type];
    }
    
    public Type GetType(string name)
    {
        return _namesToTypesInboxMap[name];
    }

    public bool CanHandleType(Type type)
    {
        return _typesToNamesInboxMap.ContainsKey(type);
    }
}