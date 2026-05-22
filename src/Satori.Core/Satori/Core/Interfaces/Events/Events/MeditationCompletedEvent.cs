namespace Satori.Core.Interfaces.Events.Events;

public sealed record MeditationCompletedEvent(int LotusId) : IGameEvent;
