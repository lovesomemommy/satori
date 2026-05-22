namespace Satori.Core.Interfaces.Events.Events;

public sealed record MeditationInterruptedEvent(int LotusId) : IGameEvent;
