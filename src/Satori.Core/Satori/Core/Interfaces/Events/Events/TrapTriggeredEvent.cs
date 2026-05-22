namespace Satori.Core.Interfaces.Events.Events;

public sealed record TrapTriggeredEvent(int SegmentIndex) : IGameEvent;
