using System;
using System.Collections.Generic;
using System.Linq;

namespace wServer.Events;

public class EventBus : IEventBus
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = new();

    public void Publish<TEvent>(TEvent @event)
    {
        if (_handlers.TryGetValue(typeof(TEvent), out var list))
            foreach (var h in list.Cast<Action<TEvent>>())
                h(@event);
    }

    public void Subscribe<TEvent>(Action<TEvent> handler)
    {
        if (!_handlers.TryGetValue(typeof(TEvent), out var list))
            _handlers[typeof(TEvent)] = list = new();
        list.Add(handler);
    }
}