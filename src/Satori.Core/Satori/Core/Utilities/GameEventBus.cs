using System;
using System.Collections.Generic;
using Satori.Core.Interfaces.Events;

namespace Satori.Core.Utilities;

public sealed class GameEventBus : IGameEventBus
{
	private readonly Dictionary<Type, List<Delegate>> _handlers = new Dictionary<Type, List<Delegate>>();

	public void Publish<TEvent>(TEvent gameEvent) where TEvent : IGameEvent
	{
		if (_handlers.TryGetValue(typeof(TEvent), out var handlers))
		{
			foreach (Delegate handler in handlers.ToArray())
			{
				((Action<TEvent>)handler)(gameEvent);
			}
		}
	}

	public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IGameEvent
	{
		var eventType = typeof(TEvent);
		if (!_handlers.TryGetValue(eventType, out var handlers))
		{
			handlers = new List<Delegate>();
			_handlers[eventType] = handlers;
		}

		handlers.Add(handler);
	}

	public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IGameEvent
	{
		if (_handlers.TryGetValue(typeof(TEvent), out var handlers))
		{
			handlers.Remove(handler);
		}
	}
}
