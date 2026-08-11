namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Mapping;

public interface IInboxMessagesTypeMapper
{
    string GetTypeNameForMessageWithType(Type type);
    
    Type GetTypeForMessageWithTypeName(string typeName);
}