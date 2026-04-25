namespace Dara.BuildingBlocks.Integration;

public class IntegrationEventBus
{
    public static IntegrationEventBus Instance => _instance == null ? _instance! : new IntegrationEventBus();
    static IntegrationEventBus? _instance;

    private readonly Dictionary<Type, List<IIntegrationEventHandler>> _handlersDict;
    
    IntegrationEventBus()
    {
        _instance = this;
        _handlersDict = new();
    }

    public void Subscribe<T>(IIntegrationEventHandler<T> handler) where T : IIntegrationEvent
    {
        Type eventType = typeof(T);
        
        if (_handlersDict.TryGetValue(eventType, out var handlers))
            handlers.Add(handler);
        else
            _handlersDict.Add(eventType, [handler]);
    }

    public async Task Publish<T>(T integrationEvent) where T : IIntegrationEvent
    {
        var handlers = _handlersDict[typeof(T)];

        foreach (var handler in handlers)
            if (handler is IIntegrationEventHandler<T> eventHandler)
                await eventHandler.HandleAsync(integrationEvent);
    }
}