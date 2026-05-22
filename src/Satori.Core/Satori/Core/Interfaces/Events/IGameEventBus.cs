using System;

namespace Satori.Core.Interfaces.Events;

public interface IGameEventBus
{
	void Publish<TEvent>(TEvent gameEvent) where TEvent : IGameEvent;

	void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IGameEvent;

	void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IGameEvent;
}
