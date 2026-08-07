namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox;

public interface IOutboxTypeMapper
{
    string GetName(Type type);
    
    Type GetType(string name);
}