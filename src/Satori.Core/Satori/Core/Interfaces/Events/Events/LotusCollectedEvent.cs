namespace Satori.Core.Interfaces.Events.Events;

public sealed record LotusCollectedEvent(int LotusId) : IGameEvent;
