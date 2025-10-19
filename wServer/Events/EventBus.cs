using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace wServer.Events;

public class EventBus : IEventBus
{
    private readonly Dictionary<Type, List<Delegate>> handlers = new();
    private readonly ILogger<EventBus> _logger;
    
    public EventBus(ILogger<EventBus> logger)
    {
        logger.LogInformation("EventBus initialized.");
        _logger = logger;
    }

    public void Publish<TEvent>(TEvent @event)
    {
        _logger.LogDebug("Publishing event of type {EventType}", typeof(TEvent).Name);
        if (handlers.TryGetValue(typeof(TEvent), out var list))
            foreach (var h in list.Cast<Action<TEvent>>())
                h(@event);
    }

    public void Subscribe<TEvent>(Action<TEvent> handler)
    {
        _logger.LogDebug("Subscribing to event of type {EventType}", typeof(TEvent).Name);
        if (!handlers.TryGetValue(typeof(TEvent), out var list))
            handlers[typeof(TEvent)] = list = new();
        list.Add(handler);
    }
}