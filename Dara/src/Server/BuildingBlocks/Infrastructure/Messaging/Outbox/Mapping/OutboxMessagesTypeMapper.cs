using Dara.Server.BuildingBlocks.Infrastructure.Common;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Mapping;

public class OutboxMessagesTypeMapper : IOutboxMessagesTypeMapper
{
    private readonly BiDictionary<Type, string> _typesNamesMap;

    public OutboxMessagesTypeMapper(BiDictionary<Type, string> typesNamesMap)
    {
        _typesNamesMap = typesNamesMap;
    }

    public string GetTypeNameForMessageWithType(Type type)
    {
        return _typesNamesMap.GetByFirst(type);
    }

    public Type GetTypeForMessageWithTypeName(string typeName)
    {
        return _typesNamesMap.GetBySecond(typeName);
    }
}
