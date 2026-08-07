namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Mapping;

public interface IOutboxTypeMapper
{
    string GetName(Type type);
    
    Type GetType(string name);
    
    bool CanHandleType(Type type);
}