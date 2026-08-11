namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Mapping;

public interface IOutboxMessagesTypeMapper
{
    string GetTypeNameForMessageWithType(Type type);
    
    Type GetTypeForMessageWithTypeName(string typeName);
}