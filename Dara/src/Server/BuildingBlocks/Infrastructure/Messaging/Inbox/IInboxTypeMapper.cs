namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox;

public interface IInboxTypeMapper
{
    string GetName(Type type);
    
    Type GetType(string name);
    
    bool CanHandleType(Type type);
}