namespace Satori.Core.Interfaces.Events.Events;

public sealed record SegmentCompletedEvent(int SegmentIndex) : IGameEvent;
